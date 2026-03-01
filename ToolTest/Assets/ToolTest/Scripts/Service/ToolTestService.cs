using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

namespace ToolTest
{
    /// <summary>
    /// Manages the service calls to Unity
    /// </summary>
    public class ToolTestService : IToolTestService
    {
        private readonly WebRequestClient client;
        private readonly AccountCredentials credentials;

        public ToolTestService()
        {
            credentials = Resources.Load<AccountCredentials>("Credentials");

            if (credentials == null)
                throw new Exception("Credentials.asset not found in Assets/ToolTest/Accounts/Credentials/Resources/");

            client = new WebRequestClient(credentials.clientId, credentials.clientSecret);
        }

        #region Player Authentication Admin API
        /// <summary>
        /// Returns all the players signed in the project
        /// </summary>
        /// <returns>List of players still not deserialized</returns>
        public async Task<JObject> ListPlayers()
        {
            try
            {
                string url = string.Format(Endpoints.PLAYER_LIST, credentials.projectId);

                string response = await client.Get(url);

                return JObject.Parse(response);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ToolTestService][ListPlayers] {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes a player registered in the project
        /// </summary>
        /// <param name="playerId">Id of the player to be deleted</param>
        /// <param name="deleteData">Optional, deletes the data saved on the CloudSave</param>
        /// <returns>The player id deleted</returns>
        public async Task<bool> DeletePlayer(string playerId, bool deleteData = true)
        {
            try
            {
                bool success = false;

                if (deleteData)
                {
                   success = await DeletePlayerCloudSaveData(playerId);
                }

                string url = string.Format(Endpoints.PLAYER_DELETE, credentials.projectId, playerId);
                success = await client.Delete(url);

                return success;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ToolTestService][DeletePlayer] {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Cloud Save Admin API

        /// <summary>
        /// Returns all the players that has data stored in the CloudSave
        /// </summary>
        /// <returns>list of player ids</returns>
        public async Task<string[]> ListPlayersFromCloudSave()
        {
            try
            {
                string url = string.Format(Endpoints.GET_PLAYERS_CLOUD_SAVE, credentials.projectId, credentials.environmentId);

                string response = await client.Get(url);

                PlayersListContent list = JsonConvert.DeserializeObject<PlayersListContent>(response);

                if(response != null)
                {
                    string[] playersList = new string[list.results.Count];
                    for(int i = 0; i < list.results.Count; i++)
                    {
                        playersList[i] = list.results[i].id;
                    }

                    return playersList;
                }
                else
                {
                    Debug.LogError("[ToolTestService][ListPlayersFromCloudSave] Data not valid");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ToolTestService][ListPlayersFromCloudSave] {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Returns a player's saved data (keys)
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns>Player keys</returns>
        public async Task<JObject> GetPlayer(string playerId)
        {
            try
            {
                string accessToken = await GetStatelessToken(credentials.clientId, credentials.clientSecret, credentials.projectId, credentials.environmentId);
                string url = string.Format(Endpoints.PLAYER_GET, credentials.projectId, credentials.environmentId, playerId);

                var headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {accessToken}" }
                };

                string response = await client.Get(url, headers);

                return JObject.Parse(response);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ToolTestService][ListPlayers] {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Creates a new player. Signs it anonimously and stores its data to the CloudSave
        /// </summary>
        /// <param name="playerContent">Player data (keys)</param>
        /// <returns>Player id created</returns>
        public async Task<string> CreatePlayer(Dictionary<string, object> playerContent)
        {
            try
            {
                await UnityServices.InitializeAsync();

                AuthenticationService.Instance.SignOut();
                AuthenticationService.Instance.ClearSessionToken();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                await CloudSaveService.Instance.Data.Player.SaveAsync(playerContent); 

                return AuthenticationService.Instance.PlayerId;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ToolTestService][CreatePlayer] {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes the data from a player saved in CloudSave
        /// </summary>
        /// <param name="playerId">Player id</param>
        /// <returns>success result</returns>
        public async Task<bool> DeletePlayerCloudSaveData(string playerId)
        {
            try
            {
                string url = string.Format(Endpoints.DELETE_PLAYER_KEYS, credentials.projectId, credentials.environmentId, playerId);
                bool success = await client.Delete(url);

                return success;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ToolTestService][DeletePlayer] {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Saves the player data
        /// </summary>
        /// <param name="playerId">Player to save data</param>
        /// <param name="data">New data</param>
        /// <returns>Result success</returns>
        public async Task<bool> SavePlayerData(string playerId, Dictionary<string, object> data)
        {
            try
            {
                bool result = false;
                foreach (string key in data.Keys)
                {
                    result = await SavePlayerItem(playerId, key, data[key]);
                    if (!result)
                    {
                        Debug.LogError($"Could not save {key} value");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ToolTestService][SavePlayerData] {ex.Message}");
                return false;
            }

        }

        /// <summary>
        /// Saves a single key from player data
        /// </summary>
        /// <param name="playerId">Player id</param>
        /// <param name="key">Key id</param>
        /// <param name="keyValue">data</param>
        /// <returns></returns>
        public async Task<bool> SavePlayerItem(string playerId, string key, object keyValue)
        {
            try
            {
                string url = string.Format(Endpoints.SAVE_PLAYER_ITEM, credentials.projectId, credentials.environmentId, playerId);
                var body = new
                {
                    key = key,
                    value = keyValue,
                };

                string bodyJson = JsonConvert.SerializeObject(body);

                var resp = await client.Post(url, bodyJson);

                if(resp != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex) 
            {
                Debug.LogError($"[ToolTestService][SavePlayerItem] {ex.Message}");
                return false;
            }
        }

        #region Access token
        public class StatelessTokenResponse
        {
            public string accessToken;
        }


        /// <summary>
        /// Gets a stateless token to acces CloudSave API services
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="secretKey"></param>
        /// <param name="projectId"></param>
        /// <param name="environmentId"></param>
        /// <returns>access token</returns>
        public async Task<string> GetStatelessToken(string keyId, string secretKey, string projectId, string environmentId)
        {
            string url = string.Format(Endpoints.STATELESS_TOKE, credentials.projectId, credentials.environmentId);

            string raw = $"{keyId}:{secretKey}";
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));

            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Basic {base64}" }
            };

            // Body must be empty for this endpoint
            string json = await client.Post(url, "", headers);

            var token = JsonConvert.DeserializeObject<StatelessTokenResponse>(json);
            return token.accessToken;
        }
        #endregion

        #endregion
    }
}
