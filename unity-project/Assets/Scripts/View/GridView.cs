using System;
using UnityEngine;
using GameGrid = Swizz.Game.Grid;

namespace Swizz.View
{
    public sealed class GridView : MonoBehaviour
    {
        private const int DefaultWidth = 32;
        private const int DefaultHeight = 24;
        private const int MinimumDimension = 1;
        private const float DefaultStepInterval = 0.2f;
        private const float MinimumStepInterval = 0.0001f;
        private const float VisibleAlpha = 1f;
        private const float HiddenAlpha = 0f;
        private const float CellZPosition = 0f;

        [SerializeField, Min(MinimumDimension)]
        private int width = DefaultWidth;

        [SerializeField, Min(MinimumDimension)]
        private int height = DefaultHeight;

        [SerializeField]
        private GameObject cellPrefab;

        [SerializeField, Min(MinimumStepInterval)]
        private float stepInterval = DefaultStepInterval;

        private GameGrid grid;
        private GameObject[] cellObjects;
        private SpriteRenderer[] cellRenderers;
        private bool subscribed;
        private float accumulatedStepTime;

        public GameGrid Grid
        {
            get
            {
                EnsureInitialized();
                return grid;
            }
        }

        public int Width => width;

        public int Height => height;

        public float StepInterval => stepInterval;

        public void Initialize(int width, int height, GameObject prefab)
        {
            if (width < MinimumDimension)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height < MinimumDimension)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            Unsubscribe();
            DestroyCellObjects();

            this.width = width;
            this.height = height;
            cellPrefab = prefab;
            grid = new GameGrid(width, height);
            cellObjects = new GameObject[width * height];
            cellRenderers = new SpriteRenderer[width * height];
            accumulatedStepTime = 0f;

            CreateCellObjects();
            Subscribe();
            Render(grid);
        }

        public void SetCell(int x, int y, bool alive)
        {
            EnsureInitialized();
            grid.Set(x, y, alive);
        }

        public void StepOnce()
        {
            EnsureInitialized();
            accumulatedStepTime = 0f;
            grid.Step();
        }

        public SpriteRenderer GetCellRenderer(int x, int y)
        {
            EnsureInitialized();
            ValidateCoordinates(x, y);
            return cellRenderers[Index(x, y)];
        }

        public bool IsCellVisible(int x, int y)
        {
            SpriteRenderer renderer = GetCellRenderer(x, y);
            return renderer != null && renderer.enabled && renderer.color.a > HiddenAlpha;
        }

        private void Start()
        {
            EnsureInitialized();
        }

        private void Update()
        {
            if (grid == null)
            {
                return;
            }

            float interval = Mathf.Max(stepInterval, MinimumStepInterval);
            accumulatedStepTime += Time.deltaTime;

            while (accumulatedStepTime >= interval)
            {
                accumulatedStepTime -= interval;
                grid.Step();
            }
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
            DestroyCellObjects();
        }

        private void EnsureInitialized()
        {
            if (grid != null)
            {
                return;
            }

            Initialize(Mathf.Max(width, MinimumDimension), Mathf.Max(height, MinimumDimension), cellPrefab);
        }

        private void CreateCellObjects()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = Index(x, y);
                    GameObject cell = CreateCellObject(x, y);
                    cellObjects[index] = cell;

                    SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();
                    if (renderer == null)
                    {
                        renderer = cell.AddComponent<SpriteRenderer>();
                    }

                    cellRenderers[index] = renderer;
                    SetRendererVisible(renderer, false);
                }
            }
        }

        private GameObject CreateCellObject(int x, int y)
        {
            GameObject cell = cellPrefab != null
                ? Instantiate(cellPrefab, transform)
                : new GameObject();

            cell.name = $"Cell_{x}_{y}";
            cell.SetActive(true);

            Transform cellTransform = cell.transform;
            cellTransform.SetParent(transform, false);
            cellTransform.localPosition = new Vector3(x, y, CellZPosition);
            cellTransform.localScale = Vector3.one;
            return cell;
        }

        private void DestroyCellObjects()
        {
            if (cellObjects == null)
            {
                return;
            }

            for (int i = 0; i < cellObjects.Length; i++)
            {
                GameObject cell = cellObjects[i];
                if (cell == null)
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(cell);
                }
                else
                {
                    DestroyImmediate(cell);
                }
            }

            cellObjects = null;
            cellRenderers = null;
        }

        private void Subscribe()
        {
            if (grid == null || subscribed || !isActiveAndEnabled)
            {
                return;
            }

            grid.GridChanged += HandleGridChanged;
            subscribed = true;
        }

        private void Unsubscribe()
        {
            if (grid == null || !subscribed)
            {
                return;
            }

            grid.GridChanged -= HandleGridChanged;
            subscribed = false;
        }

        private void HandleGridChanged(GameGrid changedGrid)
        {
            if (!ReferenceEquals(changedGrid, grid))
            {
                return;
            }

            Render(changedGrid);
        }

        private void Render(GameGrid source)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SetRendererVisible(cellRenderers[Index(x, y)], source.Get(x, y));
                }
            }
        }

        private static void SetRendererVisible(SpriteRenderer renderer, bool visible)
        {
            if (renderer == null)
            {
                return;
            }

            Color color = renderer.color;
            color.a = visible ? VisibleAlpha : HiddenAlpha;
            renderer.color = color;
            renderer.enabled = visible;
        }

        private void ValidateCoordinates(int x, int y)
        {
            if (x < 0 || x >= width)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (y < 0 || y >= height)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }
        }

        private int Index(int x, int y)
        {
            return y * width + x;
        }
    }
}
