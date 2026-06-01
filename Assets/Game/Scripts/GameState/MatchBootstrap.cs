using UnityEngine;

namespace Vitrial.GameState
{
    public sealed class MatchBootstrap : MonoBehaviour
    {
        [SerializeField] private GameModeDefinition gameMode;

        private void Awake()
        {
            if (gameMode != null)
            {
                Application.targetFrameRate = gameMode.TargetFrameRate;
            }
        }
    }
}
