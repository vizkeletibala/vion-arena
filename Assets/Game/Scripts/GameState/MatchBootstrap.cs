using UnityEngine;
using Vitrial.Networking;

namespace Vitrial.GameState
{
    public sealed class MatchBootstrap : MonoBehaviour
    {
        [SerializeField] private GameModeDefinition gameMode;

        private ClientConnectionConfig connectionConfig = ClientConnectionConfig.Localhost;
        private IClientCommandAuthorizer commandAuthorizer = new LocalPrototypeAuthorizer();

        public GameModeDefinition GameMode => gameMode;
        public AuthorityMode CurrentAuthorityMode => gameMode != null ? gameMode.AuthorityMode : AuthorityMode.LocalPrototype;
        public ClientConnectionConfig ConnectionConfig => connectionConfig;
        public IClientCommandAuthorizer CommandAuthorizer => commandAuthorizer;
        public bool ShouldAttemptClientConnection => gameMode != null && gameMode.UsesServerAuthority && gameMode.AutoConnectOnClientLaunch;

        private void Awake()
        {
            if (gameMode == null)
            {
                return;
            }

            Application.targetFrameRate = gameMode.TargetFrameRate;
            connectionConfig = gameMode.ConnectionConfig;
            commandAuthorizer = new LocalPrototypeAuthorizer();
        }
    }
}
