using UnityEngine;

namespace Gameplay
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private int maxEnemies = 10;

        public bool IsSpawning { get; private set; }
        public int CurrentEnemyCount { get; private set; }

        private float timer;

        public void Configure(GameObject prefab, float spawnInterval, int maxEnemies)
        {
            enemyPrefab = prefab;
            this.spawnInterval = spawnInterval;
            this.maxEnemies = maxEnemies;
        }

        private void Update()
        {
            if (!IsSpawning) return;

            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer = 0f;
                SpawnEnemy();
            }
        }

        public void StartSpawning()
        {
            IsSpawning = true;
            timer = 0f;
        }

        public void StopSpawning()
        {
            IsSpawning = false;
            DestroyAllEnemies();
        }

        private void SpawnEnemy()
        {
            if (CurrentEnemyCount >= maxEnemies) return;

            Vector2 spawnPos = GetRandomEdgePosition();
            var enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            if (enemy != null)
            {
                CurrentEnemyCount++;
            }
        }

        // Exposed for testability to verify cap logic without instantiating prefabs in unit tests
        public void SimulateSpawn() => SpawnEnemy();

        private void DestroyAllEnemies()
        {
            var enemies = UnityEngine.Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            foreach (var enemy in enemies)
            {
                Destroy(enemy.gameObject);
            }
            CurrentEnemyCount = 0;
        }

        private Vector2 GetRandomEdgePosition()
        {
            float x = Random.Range(-9f, 9f);
            float y = Random.Range(-5f, 5f);

            if (Random.value < 0.5f)
            {
                x = Random.value < 0.5f ? -9f : 9f;
            }
            else
            {
                y = Random.value < 0.5f ? -5f : 5f;
            }

            return new Vector2(x, y);
        }
    }
}