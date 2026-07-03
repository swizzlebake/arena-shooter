using UnityEngine;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float scorePerKill = 10f;

        public float Score { get; private set; }
        public float ScorePerKill => scorePerKill;
        public bool IsGameOver { get; private set; }

        private EnemySpawner enemySpawner;

        private void Awake()
        {
            enemySpawner = Object.FindFirstObjectByType<EnemySpawner>();
        }

        private void Start()
        {
            Score = 0f;
            IsGameOver = false;
            enemySpawner?.StartSpawning();
        }

        public void AddScore(float amount)
        {
            Score += amount;
        }

        public void OnPlayerDied()
        {
            IsGameOver = true;
            enemySpawner?.StopSpawning();
        }
    }
}
