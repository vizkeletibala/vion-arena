using UnityEngine;
using UnityEngine.SceneManagement;
using Vitrial.Enemies;

namespace Vitrial.GameState
{
    public sealed class MatchLoopController : MonoBehaviour
    {
        [SerializeField] private Health playerHealth;
        [SerializeField] private KeyCode manualRestartKey = KeyCode.Backspace;
        [SerializeField] private float restartDelaySeconds = 2.5f;

        private bool subscribed;
        private bool restarting;

        private void Start()
        {
            BindPlayerHealth();
        }

        private void Update()
        {
            if (!subscribed)
            {
                BindPlayerHealth();
            }

            if (!restarting && Input.GetKeyDown(manualRestartKey))
            {
                RestartScene();
            }
        }

        private void OnDisable()
        {
            UnbindPlayerHealth();
        }

        private void BindPlayerHealth()
        {
            if (subscribed)
            {
                return;
            }

            if (playerHealth == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerHealth = player.GetComponentInChildren<Health>();
                }
            }

            if (playerHealth != null)
            {
                playerHealth.Died += HandlePlayerDied;
                subscribed = true;
            }
        }

        private void UnbindPlayerHealth()
        {
            if (subscribed && playerHealth != null)
            {
                playerHealth.Died -= HandlePlayerDied;
            }

            subscribed = false;
        }

        private void HandlePlayerDied()
        {
            if (restarting)
            {
                return;
            }

            restarting = true;
            Invoke(nameof(RestartScene), restartDelaySeconds);
        }

        private void RestartScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.buildIndex >= 0)
            {
                SceneManager.LoadScene(activeScene.buildIndex);
            }
            else
            {
                SceneManager.LoadScene(activeScene.name);
            }
        }
    }
}
