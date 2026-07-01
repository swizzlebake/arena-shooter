using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public interface IGameOverPresenter
    {
        void Show();
        void OnRestartButtonClicked();
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int score;
        [SerializeField] private IGameOverPresenter gameOverPresenter;
        [SerializeField] private int scorePerKill = 10;

        public int Score => score;
        public bool IsGameOver { get; private set; }
        public int ScorePerKill => scorePerKill;

        public void Configure(IGameOverPresenter presenter)
        {
            gameOverPresenter = presenter;
        }

        private void Start()
        {
            AudioManager.Instance?.PlayMusic();
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
            AudioManager.Instance?.StopMusic();
            gameOverPresenter?.Show();
        }

        public void RestartGame()
        {
            gameOverPresenter?.OnRestartButtonClicked();
        }

        private void Update()
        {
            if (IsGameOver && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                RestartGame();
            }
        }
    }
}
