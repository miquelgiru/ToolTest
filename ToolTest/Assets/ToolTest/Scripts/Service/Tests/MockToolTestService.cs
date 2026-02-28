using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MockToolTestService : IToolTestService
{
    public JObject GetPlayerResult;
    public string CreateResult;
    public bool DeleteResult = true;
    public bool SaveResult = true;
    public string[] CloudSaveResult;

    // Optional: simulate exceptions
    public bool ThrowOnGetPlayer = false;
    public bool ThrowOnSavePlayerData = false;

    public Task<JObject> ListPlayers() => Task.FromResult<JObject>(null);

    public Task<JObject> GetPlayer(string playerId)
    {
        if (ThrowOnGetPlayer) throw new System.Exception("Simulated GetPlayer failure");
        return Task.FromResult(GetPlayerResult);
    }

    public Task<string> CreatePlayer(Dictionary<string, object> data) => Task.FromResult(CreateResult);

    public Task<bool> SavePlayerData(string playerId, Dictionary<string, object> data)
    {
        if (ThrowOnSavePlayerData) throw new System.Exception("Simulated SavePlayerData failure");
        return Task.FromResult(SaveResult);
    }

    public Task<bool> DeletePlayer(string playerId, bool deleteData = true) => Task.FromResult(DeleteResult);

    public Task<string[]> ListPlayersFromCloudSave() => Task.FromResult(CloudSaveResult);
}