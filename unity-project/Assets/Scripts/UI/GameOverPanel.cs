using UnityEngine;
using UnityEngine.SceneManagement;
using Gameplay;

namespace UI
{
    public class GameOverPanel : MonoBehaviour, IGameOverPresenter
    {
        [SerializeField] private GameObject panel;

        public void Configure(GameObject p) => panel = p;

        public void Show() => (panel ?? gameObject).SetActive(true);
        public void Hide() => (panel ?? gameObject).SetActive(false);

        public void OnRestartButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
