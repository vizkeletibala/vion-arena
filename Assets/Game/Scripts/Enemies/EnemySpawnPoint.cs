using UnityEngine;

namespace Vitrial.Enemies
{
    public sealed class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private bool spawnOnStart = true;

        private GameObject spawnedEnemy;

        public GameObject SpawnedEnemy => spawnedEnemy;

        private void Start()
        {
            if (spawnOnStart)
            {
                Spawn();
            }
        }

        public GameObject Spawn()
        {
            if (enemyPrefab == null)
            {
                return null;
            }

            if (spawnedEnemy != null)
            {
                return spawnedEnemy;
            }

            spawnedEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
            return spawnedEnemy;
        }
    }
}
