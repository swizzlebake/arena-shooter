using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Gameplay;

namespace UI
{
    public class GameOverPanel : MonoBehaviour, IGameOverPresenter
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private GameObject mainMenuButton;

        public void Configure(GameObject p) => panel = p;

        public void Show() => (panel ?? gameObject).SetActive(true);
        public void Hide() => (panel ?? gameObject).SetActive(false);

        private void Awake()
        {
            if (mainMenuButton != null)
            {
                var btn = mainMenuButton.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(OnMainMenuButtonClicked);
                }
            }
        }

        public void OnRestartButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnMainMenuButtonClicked()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
