using System.Collections.Generic;
using UnityEngine;

public class DummyServiceTester : MonoBehaviour
{
    public PlayersDataManager dataManager;
    public string PlayerId;


    [Space(20), Header("Mock Player Data")]
    public PlayerProfile PlayerMockData;

    [ContextMenu("Get Player")]
    async void GetPlayer()
    {
        var playerInfo = await dataManager.GetPlayer(PlayerId);

        Debug.Log(string.Format("PlayerInfo: PresetName({0}), DisplayName({1}), Level({2}), Coins({3}), ABGroup({4}), Items({5})", 
            playerInfo.PresetName, playerInfo.DisplayName, playerInfo.Level, playerInfo.Coins, playerInfo.ABGroup, playerInfo.Items.ToString()));
    }

    [ContextMenu("Create Player")]
    void CreatePlayer()
    {
        var playerData = new Dictionary<string, object> {
            { "display_name", PlayerMockData.DisplayName },
            { "preset_name", PlayerMockData.PresetName },
            { "level", PlayerMockData.Level },
            { "coins", PlayerMockData.Coins },
            { "items", PlayerMockData.Items },
            { "ab_group", PlayerMockData.ABGroup }
        };

        dataManager.CreatePlayer(playerData);
    }

    [ContextMenu("Delete Player")]
    void DeletePlayer() => dataManager.DeletePlayer(PlayerId);

    [ContextMenu("Get Players Info")]
    void GetPlayersInfo() => dataManager.GetPlayersInfo();

    [ContextMenu("Save Player Data")]
    async void SavePlayerData() => Debug.Log($"Player data saved success: {await dataManager.SavePlayerData(PlayerId, PlayerMockData)}");
}
