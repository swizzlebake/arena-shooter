using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;

        public void Configure(GameObject p) => panel = p;

        public void Show() => panel.SetActive(true);
        public void Hide() => panel.SetActive(false);

        public void OnRestartButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
