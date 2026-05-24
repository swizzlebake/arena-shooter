using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using View;

namespace InputLayer
{
    [DisallowMultipleComponent]
    public sealed class PaintInput : MonoBehaviour
    {
        [SerializeField] private GridView gridView;
        [SerializeField] private Camera worldCamera;

        private InputAction _pressAction;
        private InputAction _pointAction;
        private readonly HashSet<Vector2Int> _stroke = new HashSet<Vector2Int>();
        private bool _isPainting;
        private bool _strokeTargetAlive;
        private float _cellSize = 1f;

        private void Awake()
        {
            _pressAction = new InputAction("PaintPress", InputActionType.Button, "<Mouse>/leftButton");
            _pointAction = new InputAction("PaintPoint", InputActionType.Value, "<Mouse>/position");
            _pointAction.expectedControlType = "Vector2";

            if (worldCamera == null) worldCamera = Camera.main;
            if (gridView != null) _cellSize = ReadCellSize(gridView);
        }

        private void OnEnable()
        {
            _pressAction.started += OnPress;
            _pressAction.canceled += OnRelease;
            _pressAction.Enable();
            _pointAction.Enable();
        }

        private void OnDisable()
        {
            _pressAction.started -= OnPress;
            _pressAction.canceled -= OnRelease;
            _pressAction.Disable();
            _pointAction.Disable();
            _isPainting = false;
            _stroke.Clear();
        }

        private void OnDestroy()
        {
            _pressAction?.Dispose();
            _pointAction?.Dispose();
        }

        private void Update()
        {
            if (!_isPainting) return;
            var screen = _pointAction.ReadValue<Vector2>();
            if (TryScreenToCell(screen, out var cell)) PaintAt(cell);
        }

        private void OnPress(InputAction.CallbackContext ctx)
        {
            if (gridView == null || gridView.Model == null) return;
            _stroke.Clear();
            var screen = _pointAction.ReadValue<Vector2>();
            if (!TryScreenToCell(screen, out var cell))
            {
                _isPainting = true;
                _strokeTargetAlive = true;
                return;
            }

            _strokeTargetAlive = !gridView.Model.Get(cell.x, cell.y);
            _isPainting = true;
            PaintAt(cell);
        }

        private void OnRelease(InputAction.CallbackContext ctx)
        {
            _isPainting = false;
            _stroke.Clear();
        }

        private bool TryScreenToCell(Vector2 screenPos, out Vector2Int cell)
        {
            cell = default;
            if (gridView == null) return false;
            if (worldCamera == null) worldCamera = Camera.main;
            if (worldCamera == null) return false;

            var screen3 = new Vector3(screenPos.x, screenPos.y, -worldCamera.transform.position.z);
            var world = worldCamera.ScreenToWorldPoint(screen3);
            var local = world - gridView.transform.position;
            int x = Mathf.FloorToInt(local.x / _cellSize);
            int y = Mathf.FloorToInt(local.y / _cellSize);
            if (x < 0 || x >= gridView.Width || y < 0 || y >= gridView.Height) return false;
            cell = new Vector2Int(x, y);
            return true;
        }

        private void PaintAt(Vector2Int cell)
        {
            if (_stroke.Contains(cell)) return;
            _stroke.Add(cell);
            gridView.Model.Set(cell.x, cell.y, _strokeTargetAlive);
        }

        private static float ReadCellSize(GridView view)
        {
            var field = typeof(GridView).GetField("cellSize",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic);
            if (field == null) return 1f;
            var value = field.GetValue(view);
            return value is float f && f > 0f ? f : 1f;
        }
    }
}
