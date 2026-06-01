using UnityEngine;

namespace Vitrial.GameState
{
    [CreateAssetMenu(menuName = "Vitrial/Game Mode", fileName = "GameModeDefinition")]
    public sealed class GameModeDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Prototype Arena";
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private float matchDurationSeconds = 600f;

        public string DisplayName => displayName;
        public int TargetFrameRate => targetFrameRate;
        public float MatchDurationSeconds => matchDurationSeconds;
    }
}
