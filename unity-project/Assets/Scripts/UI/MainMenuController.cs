using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Gameplay;

namespace UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject playButton;
        [SerializeField] private GameObject quitButton;

        private void Awake()
        {
            if (playButton != null)
            {
                var btn = playButton.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(PlayGame);
                }
            }
            if (quitButton != null)
            {
                var btn = quitButton.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(QuitGame);
                }
            }
        }

        private void PlayGame()
        {
            SceneManager.LoadScene("Main");
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
