using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace ToolTest
{
    public class ToolTestService
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

        public async Task<string> CreatePlayer()
        {
            try
            {
                await UnityServices.InitializeAsync();

                AuthenticationService.Instance.SignOut();
                AuthenticationService.Instance.ClearSessionToken();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                return AuthenticationService.Instance.PlayerId;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ToolTestService][CreatePlayer] {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeletePlayer(string playerId)
        {
            try
            {
                string url = string.Format(Endpoints.PLAYER_DELETE, credentials.projectId, playerId);

                bool success = await client.Delete(url);

                return success;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ToolTestService][DeletePlayer] {ex.Message}");
                throw;
            }
        }
    }
}
