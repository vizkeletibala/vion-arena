using UnityEngine;
using Vitrial.Networking;

namespace Vitrial.GameState
{
    [CreateAssetMenu(menuName = "Vitrial/Game Mode", fileName = "GameModeDefinition")]
    public sealed class GameModeDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Prototype Arena";
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private float matchDurationSeconds = 600f;

        [Header("Authority + Connection Prep")]
        [SerializeField] private AuthorityMode authorityMode = AuthorityMode.LocalPrototype;
        [SerializeField] private string serverHost = "127.0.0.1";
        [SerializeField] private int serverPort = 7777;
        [SerializeField] private bool secureTransport;
        [SerializeField] private bool autoConnectOnClientLaunch;
        [SerializeField] private string dedicatedServerBuildTarget = "LinuxServer";

        public string DisplayName => displayName;
        public int TargetFrameRate => targetFrameRate;
        public float MatchDurationSeconds => matchDurationSeconds;
        public AuthorityMode AuthorityMode => authorityMode;
        public string ServerHost => serverHost;
        public int ServerPort => serverPort;
        public bool SecureTransport => secureTransport;
        public bool AutoConnectOnClientLaunch => autoConnectOnClientLaunch;
        public string DedicatedServerBuildTarget => dedicatedServerBuildTarget;
        public ClientConnectionConfig ConnectionConfig => new ClientConnectionConfig(serverHost, serverPort, secureTransport, displayName);
        public bool UsesServerAuthority => authorityMode == AuthorityMode.ServerAuthoritative;
    }
}
