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

        private bool _subscribed;

        private void OnEnable()
        {
            TrySubscribe();
        }

        private void Start()
        {
            // GridView.Awake constructs the Model; ordering between two MonoBehaviours'
            // OnEnable is undefined, so the initial label refresh happens in Start.
            TrySubscribe();
            RefreshLabelFromModel();
            RefreshInteractable();
        }

        private void OnDisable()
        {
            if (_subscribed && gridView != null)
            {
                gridView.GenerationChanged -= HandleGenerationChanged;
                _subscribed = false;
            }
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
            if (!gridView.IsPlaying)
            {
                gridView.StepOnce();
            }
            RefreshInteractable();
        }

        public void OnResetClicked()
        {
            if (gridView == null) return;
            gridView.ResetGrid();
            RefreshLabelFromModel();
            RefreshInteractable();
        }

        private void TrySubscribe()
        {
            if (_subscribed || gridView == null) return;
            gridView.GenerationChanged += HandleGenerationChanged;
            _subscribed = true;
        }

        private void RefreshLabelFromModel()
        {
            int gen = (gridView != null && gridView.Model != null) ? gridView.Model.Generation : 0;
            HandleGenerationChanged(gen);
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
