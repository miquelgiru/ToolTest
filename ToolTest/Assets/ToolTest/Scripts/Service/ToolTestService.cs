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

        #region Access token
        public class StatelessTokenResponse
        {
            public string accessToken;
        }

        public async Task<string> GetStatelessToken(string keyId, string secretKey, string projectId, string environmentId)
        {
            string url = string.Format(Endpoints.STATELESS_TOKE, credentials.projectId,credentials.environmentId);

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

        public async Task<bool> DeletePlayer(string playerId, bool deleteData = true)
        {
            try
            {
                bool success = await DeletePlayerCloudSaveData(playerId);
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
    }
}
