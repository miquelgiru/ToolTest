using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToolTest;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayersDataManager : MonoBehaviour
{
    private IToolTestService service;
    private readonly Dictionary<string, PlayerProfile> playersInfo = new();

    public void Initialize(IToolTestService injectedService)
    {
        service = injectedService;
    }

    private void Awake()
    {
        if (service == null)
            service = new ToolTestService();
    }

    public async Task<PlayerProfile> GetPlayer(string playerID)
    {
        if (playersInfo.TryGetValue(playerID, out var cached))
            return cached;

        JObject result = await service.GetPlayer(playerID);
        if (result == null) return null;

        var resultData = result.ToObject<PlayerDataContent>();
        var info = ConvertRawDataIntoPlayerProfile(resultData.results);

        if (info != null)
            playersInfo[playerID] = info;

        return info;
    }

    public async Task<bool> CreatePlayer(Dictionary<string, object> playerData)
    {
        string result = await service.CreatePlayer(playerData);
        return !string.IsNullOrEmpty(result);
    }

    public async Task<bool> DeletePlayer(string playerId)
    {
        bool result = await service.DeletePlayer(playerId);

        if (result)
            playersInfo.Remove(playerId);

        return result;
    }

    public async Task<string[]> GetPlayersInfo()
    {
        var result = await service.ListPlayersFromCloudSave();

        if (result == null || result.Length == 0)
        {
            Debug.LogWarning("[PlayersDataManager][GetPlayersInfo] No players found in Cloud Save");
            return new string[0];
        }

        return result;
    }

    private PlayerProfile ConvertRawDataIntoPlayerProfile(List<PlayerDataItem> data)
    {
        var playerInfo = new PlayerProfile();

        foreach (var item in data)
        {
            switch (item.key)
            {
                case "preset_name":
                    playerInfo.PresetName = item.value as string;
                    break;
                case "display_name":
                    playerInfo.DisplayName = item.value as string;
                    break;
                case "level":
                    if (int.TryParse(item.value.ToString(), out int level))
                        playerInfo.Level = level;
                    else
                        Debug.LogError($"[PlayersDataManager] Invalid level value: {item.value}");
                    break;
                case "coins":
                    if (int.TryParse(item.value.ToString(), out int coins))
                        playerInfo.Coins = coins;
                    else
                        Debug.LogError($"[PlayersDataManager] Invalid coins value: {item.value}");
                    break;
                case "ab_group":
                    playerInfo.ABGroup = item.value as string;
                    break;
                case "items":
                    try
                    {
                        string[] parsedItems = JsonConvert.DeserializeObject<string[]>(item.value.ToString());
                        if (parsedItems != null)
                            playerInfo.Items = parsedItems;
                        else
                            Debug.LogError("[PlayersDataManager] Items list is null or invalid");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[PlayersDataManager] Exception parsing items: {ex.Message}");
                    }
                    break;
                default:
                    Debug.LogWarning($"[PlayersDataManager] Unknown key in player data: {item.key}");
                    break;
            }
        }

        return playerInfo;
    }

    public async Task<bool> SavePlayerData(string playerId, PlayerProfile playerInfo)
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

        if (!PlayerDataValidator.ValidateDictionary(savedData, out _))
            return false;

        bool result = await service.SavePlayerData(playerId, savedData);

        if (result)
            playersInfo.Remove(playerId);

        return result;
    }

    public async Task<Dictionary<string, PlayerProfile>> GetPlayersProfileData()
    {
        string[] palyerIds = await GetPlayersInfo();

        foreach (string playerId in palyerIds)
        {
            PlayerProfile playerData = await GetPlayer(playerId);
            playersInfo[playerId] = playerData;
        }

        return playersInfo;
    }

    // For testing visibility (read-only)
    public bool IsCached(string playerId) => playersInfo.ContainsKey(playerId);
}
