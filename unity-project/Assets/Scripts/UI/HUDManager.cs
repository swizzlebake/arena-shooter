using UnityEngine;
using Gameplay;

namespace UI
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private HUDScoreDisplay scoreDisplay;
        [SerializeField] private GameOverPanel gameOverPanel;

        private void Start()
        {
            if (scoreDisplay != null)
                scoreDisplay.Configure(gameManager);
        }
    }
}
