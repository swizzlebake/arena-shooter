using TMPro;
using UnityEngine;

namespace UI
{
    public class HUDScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private GameManager gameManager;

        public void Configure(GameManager manager) => gameManager = manager;

        private void Update()
        {
            if (gameManager != null && scoreText != null)
            {
                scoreText.text = gameManager.Score.ToString();
            }
        }
    }
}
