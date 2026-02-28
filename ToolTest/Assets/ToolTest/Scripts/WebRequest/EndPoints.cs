namespace ToolTest
{
    public static class Endpoints
    {
        // Player Management
        public const string PLAYER_LIST = "https://services.api.unity.com/player-identity/v1/projects/{0}/users";
        public const string PLAYER_GET = "https://services.api.unity.com/cloud-save/v1/data/projects/{0}/environments/{1}/players/{2}/items";
        public const string PLAYER_DELETE = "https://services.api.unity.com/player-identity/v1/projects/{0}/users/{1}";
        public const string PLAYER_CREATE = "https://services.api.unity.com/auth/v1/projects/{0}/players/sign-in";

        // Cloud Save
        public const string ACCESS_TOKEN = "https://services.api.unity.com/auth/v1/token";
        public const string STATELESS_TOKE = "https://services.api.unity.com/auth/v1/token-exchange?projectId={0}&environmentId={1}";
        public const string GET_PLAYERS_CLOUD_SAVE = "https://services.api.unity.com/cloud-save/v1/data/projects/{0}/environments/{1}/players";
        public const string SAVE_PLAYER_ITEM = "https://services.api.unity.com/cloud-save/v1/data/projects/{0}/environments/{1}/players/{2}/items";
        public const string DELETE_PLAYER_KEYS = "https://services.api.unity.com/cloud-save/v1/data/projects/{0}/environments/{1}/players/{2}/items";
    }
}

