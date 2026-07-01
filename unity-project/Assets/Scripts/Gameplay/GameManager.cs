using UnityEngine;
using UnityEngine.InputSystem;
using UI;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int score;
        [SerializeField] private GameOverPanel gameOverPanel;
        [SerializeField] private int scorePerKill = 10;

        public int Score => score;
        public bool IsGameOver { get; private set; }
        public int ScorePerKill => scorePerKill;

        public void Configure(GameOverPanel panel)
        {
            gameOverPanel = panel;
        }

        public void AddScore(int amount)
        {
            if (IsGameOver) return;
            score += amount;
        }

        public void TriggerGameOver()
        {
            if (IsGameOver) return;
            IsGameOver = true;
            gameOverPanel?.Show();
        }

        private void Update()
        {
            if (IsGameOver && Keyboard.current.rKey.wasPressedThisFrame)
            {
                gameOverPanel?.OnRestartButtonClicked();
            }
        }
    }
}
