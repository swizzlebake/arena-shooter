using UnityEngine;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int score;

        public int Score => score;
        public bool IsGameOver { get; private set; }

        public void AddScore(int amount)
        {
            if (IsGameOver) return;
            score += amount;
        }

        public void TriggerGameOver()
        {
            if (IsGameOver) return;
            IsGameOver = true;
        }
    }
}
