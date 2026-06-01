using UnityEngine;
using UnityEngine.UI;
using Vitrial.Enemies;
using Vitrial.Weapons;

namespace Vitrial.UI
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private Text healthText;
        [SerializeField] private Text ammoText;
        [SerializeField] private Text reloadText;
        [SerializeField] private Health health;
        [SerializeField] private StarterRifleController weapon;

        private void Awake()
        {
            EnsureFallbackText(ref healthText, "HealthText", new Vector2(18f, -18f), "HP --/--");
            EnsureFallbackText(ref ammoText, "AmmoText", new Vector2(18f, -46f), "Ammo --/--");
            EnsureFallbackText(ref reloadText, "ReloadText", new Vector2(18f, -74f), string.Empty);

            if (health == null)
            {
                foreach (Health candidate in FindObjectsOfType<Health>())
                {
                    if (candidate.CompareTag("Player"))
                    {
                        health = candidate;
                        break;
                    }

                    if (health == null)
                    {
                        health = candidate;
                    }
                }
            }

            if (weapon == null)
            {
                weapon = FindObjectOfType<StarterRifleController>();
            }
        }

        private void EnsureFallbackText(ref Text field, string objectName, Vector2 anchoredPosition, string defaultText)
        {
            if (field != null)
            {
                return;
            }

            GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(transform, false);

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(360f, 28f);

            field = textObject.GetComponent<Text>();
            field.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            field.fontSize = 20;
            field.color = Color.white;
            field.text = defaultText;
        }

        private void OnEnable()
        {
            Subscribe();
            RefreshHealth();
            RefreshAmmo();
            SetReloading(false);
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (health != null)
            {
                health.Damaged += OnDamaged;
                health.Died += OnDied;
            }

            if (weapon != null)
            {
                weapon.AmmoChanged += OnAmmoChanged;
                weapon.ReloadStarted += OnReloadStarted;
                weapon.ReloadCompleted += OnReloadCompleted;
            }
        }

        private void Unsubscribe()
        {
            if (health != null)
            {
                health.Damaged -= OnDamaged;
                health.Died -= OnDied;
            }

            if (weapon != null)
            {
                weapon.AmmoChanged -= OnAmmoChanged;
                weapon.ReloadStarted -= OnReloadStarted;
                weapon.ReloadCompleted -= OnReloadCompleted;
            }
        }

        private void OnDamaged(DamagePayload payload)
        {
            RefreshHealth();
        }

        private void OnDied()
        {
            RefreshHealth();
        }

        private void OnAmmoChanged(WeaponAmmoState state)
        {
            RefreshAmmo();
        }

        private void OnReloadStarted()
        {
            SetReloading(true);
        }

        private void OnReloadCompleted()
        {
            SetReloading(false);
            RefreshAmmo();
        }

        private void RefreshHealth()
        {
            if (healthText == null)
            {
                return;
            }

            if (health == null)
            {
                healthText.text = "HP --/--";
                return;
            }

            healthText.text = $"HP {Mathf.CeilToInt(health.CurrentHealth)}/{Mathf.CeilToInt(health.MaxHealth)}";
        }

        private void RefreshAmmo()
        {
            if (ammoText == null)
            {
                return;
            }

            WeaponAmmoState ammo = weapon != null ? weapon.AmmoState : null;
            if (ammo == null)
            {
                ammoText.text = "Ammo --/--";
                return;
            }

            ammoText.text = $"Ammo {ammo.CurrentMagazine}/{ammo.ReserveAmmo}";
        }

        private void SetReloading(bool isReloading)
        {
            if (reloadText != null)
            {
                reloadText.text = isReloading ? "Reloading" : string.Empty;
            }
        }
    }
}
