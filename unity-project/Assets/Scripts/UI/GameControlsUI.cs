using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View;

namespace UI
{
    [DisallowMultipleComponent]
    public sealed class GameControlsUI : MonoBehaviour
    {
        [SerializeField] private GridView gridView;
        [SerializeField] private Button playButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button stepButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private TMP_Text generationLabel;
        [SerializeField] private string generationFormat = "Generation: {0}";

        private void OnEnable()
        {
            if (gridView != null) gridView.GenerationChanged += HandleGenerationChanged;
            if (gridView != null && gridView.Model != null) HandleGenerationChanged(gridView.Model.Generation);
            else HandleGenerationChanged(0);
            RefreshInteractable();
        }

        private void OnDisable()
        {
            if (gridView != null) gridView.GenerationChanged -= HandleGenerationChanged;
        }

        public void OnPlayClicked()
        {
            if (gridView == null) return;
            gridView.Play();
            RefreshInteractable();
        }

        public void OnPauseClicked()
        {
            if (gridView == null) return;
            gridView.Pause();
            RefreshInteractable();
        }

        public void OnStepClicked()
        {
            if (gridView == null) return;
            if (gridView.IsPlaying) return;
            gridView.StepOnce();
            RefreshInteractable();
        }

        public void OnResetClicked()
        {
            if (gridView == null) return;
            gridView.ResetGrid();
            if (gridView.Model != null) HandleGenerationChanged(gridView.Model.Generation);
            RefreshInteractable();
        }

        private void HandleGenerationChanged(int generation)
        {
            if (generationLabel == null) return;
            generationLabel.text = string.Format(generationFormat, generation);
        }

        private void RefreshInteractable()
        {
            bool playing = gridView != null && gridView.IsPlaying;
            if (playButton != null) playButton.interactable = !playing;
            if (pauseButton != null) pauseButton.interactable = playing;
            if (stepButton != null) stepButton.interactable = !playing;
            if (resetButton != null) resetButton.interactable = true;
        }
    }
}
