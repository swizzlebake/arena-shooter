using UnityEngine;
using Game;

namespace Gameplay
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private WavePlan wavePlan;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private int maxEnemies = 10;

        public bool IsSpawning { get; private set; }
        public int CurrentEnemyCount { get; private set; }

        private int currentWave;
        public int CurrentWave => currentWave;

        private float timer;
        private int spawnedInCurrentWave;
        private float interWaveTimer;

        public void Configure(GameObject prefab, float spawnInterval, int maxEnemies)
        {
            enemyPrefab = prefab;
            this.spawnInterval = spawnInterval;
            this.maxEnemies = maxEnemies;
        }

        public void Configure(WavePlan plan)
        {
            wavePlan = plan;
        }

        private void Update()
        {
            if (!IsSpawning) return;

            if (wavePlan != null && IsWaveComplete())
            {
                if (wavePlan.HasNextWave(currentWave))
                {
                    interWaveTimer += Time.deltaTime;
                    if (interWaveTimer >= wavePlan[currentWave].InterWaveDelay)
                    {
                        AdvanceWave();
                    }
                    return;
                }

                IsSpawning = false;
                return;
            }

            timer += Time.deltaTime;
            float interval = wavePlan != null ? wavePlan[currentWave].SpawnInterval : spawnInterval;
            if (timer >= interval)
            {
                timer = 0f;
                SpawnEnemy();
            }
        }

        public void StartSpawning()
        {
            IsSpawning = true;
            timer = 0f;
            currentWave = 0;
            spawnedInCurrentWave = 0;
            interWaveTimer = 0f;
        }

        public void StopSpawning()
        {
            IsSpawning = false;
            DestroyAllEnemies();
        }

        public void OnEnemyKilled()
        {
            CurrentEnemyCount = Mathf.Max(0, CurrentEnemyCount - 1);
        }

        private void SpawnEnemy()
        {
            if (CurrentEnemyCount >= maxEnemies) return;

            if (wavePlan != null && spawnedInCurrentWave >= wavePlan[currentWave].EnemiesToSpawn)
                return;

            Vector2 spawnPos = GetRandomEdgePosition();

            GameObject enemyObj;
            if (ObjectPoolManager.Instance != null)
            {
                enemyObj = ObjectPoolManager.Instance.CheckoutEnemy(spawnPos);
            }
            else
            {
                enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            }

            if (enemyObj != null)
            {
                CurrentEnemyCount++;
                spawnedInCurrentWave++;
            }
        }

        public void SimulateSpawn() => SpawnEnemy();

        private bool IsWaveComplete()
        {
            if (wavePlan == null) return false;

            WaveConfig wave = wavePlan[currentWave];
            return spawnedInCurrentWave >= wave.EnemiesToSpawn && CurrentEnemyCount == 0;
        }

        private void AdvanceWave()
        {
            currentWave++;
            spawnedInCurrentWave = 0;
            interWaveTimer = 0f;
        }

        private void DestroyAllEnemies()
        {
            var enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            foreach (var enemy in enemies)
            {
                if (ObjectPoolManager.Instance != null)
                {
                    ObjectPoolManager.Instance.ReturnEnemy(enemy.gameObject);
                }
                else
                {
                    Destroy(enemy.gameObject);
                }
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
