using Newtonsoft.Json.Linq;
using ToolTest;
using UnityEngine;

public class PlayersDataManager : MonoBehaviour
{
    private ToolTestService service;

    private void Awake()
    {
        service = new ToolTestService();
    }

    public async void ListPlayers()
    {
        try
        {
            JObject result = await service.ListPlayers();
            Debug.Log($"Players:\n{result.ToString()}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ToolTestBehaviour][ListPlayers] {ex.Message}");
        }
    }

    public async void CreatePlayer()
    {
        try
        {
            string result = await service.CreatePlayer();
            Debug.Log($"Created Player:\n{result.ToString()}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ToolTestBehaviour][CreatePlayer] {ex.Message}");
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
            Debug.LogError($"[ToolTestBehaviour][DeletePlayer] {ex.Message}");
        }
    }
}
