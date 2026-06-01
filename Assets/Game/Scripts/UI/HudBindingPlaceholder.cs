using UnityEngine;

namespace Vitrial.UI
{
    public sealed class HudBindingPlaceholder : MonoBehaviour
    {
        [SerializeField] private Canvas rootCanvas;

        public Canvas RootCanvas => rootCanvas;
    }
}
