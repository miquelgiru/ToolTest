namespace ToolTest
{
    public static class Endpoints
    {
        // Player Management
        public const string PLAYER_LIST = "https://services.api.unity.com/player-identity/v1/projects/{0}/users";
        public const string PLAYER_DELETE = "https://services.api.unity.com/player-identity/v1/projects/{0}/users/{1}";
        public const string PLAYER_CREATE = "https://services.api.unity.com/auth/v1/projects/{0}/players/sign-in";

        // Cloud Save
        public const string CLOUDSAVE_PLAYER_DATA = "https://services.unity.com/cloud-save/v1/projects/{0}/players/{1}/data";
    }
}

