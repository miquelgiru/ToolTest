using UnityEngine;

namespace ToolTest
{
    [CreateAssetMenu(fileName = "ToolTest", menuName = "ToolTest/Account Credentials")]
    public class AccountCredentials : ScriptableObject
    {
        public string clientId;
        public string clientSecret;
        public string projectId;
        public string environmentId;
    }
}

