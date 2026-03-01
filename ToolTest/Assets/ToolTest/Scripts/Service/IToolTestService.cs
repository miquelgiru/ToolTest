using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IToolTestService
{
    /// <summary>
    /// Returns all the players signed in the project
    /// </summary>
    /// <returns>List of players still not deserialized</returns>
    Task<JObject> ListPlayers();

    /// <summary>
    /// Returns a player's saved data (keys)
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns>Player keys</returns>
    Task<JObject> GetPlayer(string playerId);

    /// <summary>
    /// Creates a new player. Signs it anonimously and stores its data to the CloudSave
    /// </summary>
    /// <param name="playerContent">Player data (keys)</param>
    /// <returns>Player id created</returns>
    Task<string> CreatePlayer(Dictionary<string, object> data);

    /// <summary>
    /// Saves the player data
    /// </summary>
    /// <param name="playerId">Player to save data</param>
    /// <param name="data">New data</param>
    /// <returns>Result success</returns>
    Task<bool> SavePlayerData(string playerId, Dictionary<string, object> data);

    /// <summary>
    /// Deletes a player registered in the project
    /// </summary>
    /// <param name="playerId">Id of the player to be deleted</param>
    /// <param name="deleteData">Optional, deletes the data saved on the CloudSave</param>
    /// <returns>The player id deleted</returns>
    Task<bool> DeletePlayer(string playerId, bool deleteData = true);

    /// <summary>
    /// Returns all the players that has data stored in the CloudSave
    /// </summary>
    /// <returns>list of player ids</returns>
    Task<string[]> ListPlayersFromCloudSave();
}