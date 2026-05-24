using UnityEngine;

namespace View
{
    [DisallowMultipleComponent]
    public sealed class CellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public bool IsAlive { get; private set; }

        public float CurrentAlpha => spriteRenderer != null ? spriteRenderer.color.a : 0f;

        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetAlive(bool alive, bool animate)
        {
            IsAlive = alive;
            if (spriteRenderer == null) return;
            // Task 4 will extend with fade/scale; for now apply final state.
            var c = spriteRenderer.color;
            c.a = alive ? 1f : 0f;
            spriteRenderer.color = c;
            transform.localScale = Vector3.one;
        }
    }
}
