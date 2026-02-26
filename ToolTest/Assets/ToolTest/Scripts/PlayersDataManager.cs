using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToolTest;
using UnityEngine;

public class PlayersDataManager : MonoBehaviour
{
    private ToolTestService service;
    private Dictionary<string, PlayerProfile> playersInfo = new Dictionary<string, PlayerProfile>();

    private void Awake()
    {
        service = new ToolTestService();
    }

    public async Task<PlayersListInfo> ListPlayers()
    {
        try
        {
            JObject result = await service.ListPlayers();
            Debug.Log($"Players:{result.ToString()}");

            return null;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ToolTestBehaviour][ListPlayers] {ex.Message}");
            return null;
        }
    }

    public async Task<PlayerProfile> GetPlayer(string playerID)
    {
        try
        {
            if (playersInfo.ContainsKey(playerID))
            {
                return playersInfo[playerID];
            }

            JObject result = await service.GetPlayer(playerID);

            if (result != null)
            {
                var resultData = result.ToObject<PlayerDataContent>();
                PlayerProfile info = ConvertPlayerDataIntoPlayerInfo(resultData.results);

                if (info != null)
                {
                    playersInfo.Add(playerID, info);
                    return info;
                }
                else
                {
                    Debug.LogError($"[PlayersDataManager][GetPlayer] Error parsing player data");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"[PlayersDataManager][GetPlayer] Response empty");
                return null;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[PlayersDataManager][GetPlayer] {ex.Message}");
            return null;
        }
    }

    public async void CreatePlayer()
    {
        try
        {
            var data = new Dictionary<string, object> { 
                { "display_name", "Endgame Whale" },
                { "preset_name", "QA_Whale" },
                { "level", "100" },
                { "coins", "999999" },
                { "items", new string[]{ "sling_power_3", "extra_bird_2", "king_sling", "tnt_drop", "speed_boost" } },
                { "ab_group", "variant_b" }
            };

            string result = await service.CreatePlayer(data);
            Debug.Log($"Created Player:\n{result.ToString()}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[PlayersDataManager][CreatePlayer] {ex.Message}");
        }
    }

    public async void DeletePlayer(string playerId)
    {
        try
        {
            bool result = await service.DeletePlayer(playerId);
            Debug.Log($"Deleted Player {playerId}:\n Success: {result.ToString()}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[PlayersDataManager][DeletePlayer] {ex.Message}");
        }
    }

    public async void GetPlayersInfo()
    {
        try
        {
            JObject result = await service.ListPlayersFromCloudSave();
            Debug.Log($"Players:\n{result.ToString()}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[PlayersDataManager][ListPlayers] {ex.Message}");           
        }
    }

    private PlayerProfile ConvertPlayerDataIntoPlayerInfo(List<PlayerDataItem> data)
    {
        PlayerProfile playerInfo = new PlayerProfile();

        foreach (PlayerDataItem item in data)
        {
            if (item.key.Equals("preset_name"))
            {
                playerInfo.PresetName = item.value as string;
            }
            else if (item.key.Equals("display_name"))
            {
                playerInfo.DisplayName = item.value as string;
            }
            else if (item.key.Equals("level"))
            {
                playerInfo.Level = Convert.ToInt32(item.value);
            }
            else if (item.key.Equals("coins"))
            {
                playerInfo.Coins = Convert.ToInt32(item.value);
            }
            else if (item.key.Equals("ab_group"))
            {
                playerInfo.ABGroup = item.value as string;
            }
            else if (item.key.Equals("items"))
            {
                string rawItemsData = item.value.ToString();
                string[] parsedItems = JsonConvert.DeserializeObject<string[]>(rawItemsData);

                if(parsedItems != null)
                {
                    playerInfo.Items = parsedItems;
                }
                else
                {
                    Debug.LogError("[PlayersDataManager][ConvertPlayerDataIntoPlayerInfo] Error obtaining items list");
                }
            }
        }

        return playerInfo;
    }

    public async Task<bool> SavePlayerData(string playerId, PlayerProfile playerInfo)
    {
        try
        {
            var savedData = new Dictionary<string, object>
            {
                { "preset_name", playerInfo.PresetName },
                { "display_name", playerInfo.DisplayName },
                { "level", playerInfo.Level },
                { "coins", playerInfo.Coins },
                { "ab_group", playerInfo.ABGroup },
                { "items", playerInfo.Items },
            };

            string error = string.Empty;
            if(!PlayerDataValidator.ValidateDictionary(savedData,out error))
            {
                Debug.LogError($"[PlayersDataManager][SavePlayerData] Invalid data: {error}");
                return false;
            }

            bool result = await service.SavePlayerData(playerId, savedData);

            if (result)
            {
                playersInfo.Remove(playerId);
            }

            return result;
        }
        catch(System.Exception ex)
        {
            Debug.LogError($"[PlayersDataManager][SavePlayerData] {ex.Message}");
            return false;
        }
        
    }
}
