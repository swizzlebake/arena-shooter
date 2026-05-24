using System;
using UnityEngine;

namespace View
{
    [DisallowMultipleComponent]
    public sealed class GridView : MonoBehaviour
    {
        private const float AnimateThresholdSeconds = 0.15f;

        [SerializeField] private int gridWidth = 32;
        [SerializeField] private int gridHeight = 24;
        [SerializeField] private CellView cellPrefab;
        [SerializeField] private float stepInterval = 0.2f;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Transform cellRoot;

        private CellView[,] _cellViews;
        private float _stepAccumulator;

        public Game.Grid Model { get; private set; }
        public bool IsPlaying { get; private set; }
        public int Width => gridWidth;
        public int Height => gridHeight;

        public event Action<int> GenerationChanged;

        private void Awake()
        {
            Model = new Game.Grid(gridWidth, gridHeight);
            BuildCells();
            Model.GridChanged += OnGridChanged;
            OnGridChanged();
        }

        private void OnDestroy()
        {
            if (Model != null) Model.GridChanged -= OnGridChanged;
        }

        private void Update()
        {
            if (!IsPlaying) return;
            _stepAccumulator += Time.deltaTime;
            if (_stepAccumulator >= stepInterval)
            {
                _stepAccumulator -= stepInterval;
                Model.Step();
            }
        }

        public CellView GetCellView(int x, int y)
        {
            if (_cellViews == null) return null;
            if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) return null;
            return _cellViews[x, y];
        }

        public void Play()
        {
            IsPlaying = true;
            _stepAccumulator = 0f;
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void StepOnce()
        {
            Model.Step();
        }

        public void ResetGrid()
        {
            IsPlaying = false;
            _stepAccumulator = 0f;
            Model.Clear();
        }

        private void BuildCells()
        {
            if (cellPrefab == null)
            {
                Debug.LogError("GridView: cellPrefab is not assigned.", this);
                return;
            }

            if (cellRoot == null)
            {
                var rootGo = new GameObject("CellRoot");
                rootGo.transform.SetParent(transform, false);
                cellRoot = rootGo.transform;
            }

            _cellViews = new CellView[gridWidth, gridHeight];
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    var cell = Instantiate(cellPrefab, cellRoot);
                    cell.name = $"Cell_{x}_{y}";
                    cell.transform.localPosition = new Vector3(x * cellSize, y * cellSize, 0f);
                    cell.SetAlive(false, false);
                    _cellViews[x, y] = cell;
                }
            }
        }

        private void OnGridChanged()
        {
            if (_cellViews == null) return;
            bool animate = stepInterval >= AnimateThresholdSeconds;
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    bool alive = Model.Get(x, y);
                    var view = _cellViews[x, y];
                    if (view == null) continue;
                    if (view.IsAlive == alive) continue;
                    view.SetAlive(alive, animate);
                }
            }
            GenerationChanged?.Invoke(Model.Generation);
        }
    }
}
