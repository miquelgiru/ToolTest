using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IToolTestService
{
    Task<JObject> ListPlayers();
    Task<JObject> GetPlayer(string playerId);
    Task<string> CreatePlayer(Dictionary<string, object> data);
    Task<bool> SavePlayerData(string playerId, Dictionary<string, object> data);
    Task<bool> DeletePlayer(string playerId, bool deleteData = true);
    Task<string[]> ListPlayersFromCloudSave();
}