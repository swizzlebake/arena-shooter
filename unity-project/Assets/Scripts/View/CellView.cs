using UnityEngine;

namespace View
{
    [DisallowMultipleComponent]
    public sealed class CellView : MonoBehaviour
    {
        private const float TransitionDuration = 0.15f;

        private enum Phase { Idle, Birth, Death }

        [SerializeField] private SpriteRenderer spriteRenderer;

        private Phase _phase = Phase.Idle;
        private float _elapsed;

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

            if (!animate)
            {
                SnapTo(alive);
                return;
            }

            if (alive)
            {
                if (_phase == Phase.Birth) return;
                if (CurrentAlpha >= 1f && transform.localScale.x >= 1f)
                {
                    _phase = Phase.Idle;
                    return;
                }
                _phase = Phase.Birth;
                _elapsed = 0f;
                SetAlpha(0f);
                transform.localScale = Vector3.one;
            }
            else
            {
                if (_phase == Phase.Death) return;
                if (CurrentAlpha <= 0f)
                {
                    _phase = Phase.Idle;
                    SnapTo(false);
                    return;
                }
                _phase = Phase.Death;
                _elapsed = 0f;
                SetAlpha(1f);
                transform.localScale = Vector3.one;
            }
        }

        private void Update()
        {
            if (_phase == Phase.Idle) return;

            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / TransitionDuration);

            if (_phase == Phase.Birth)
            {
                float eased = 1f - (1f - t) * (1f - t);
                SetAlpha(eased);
                if (t >= 1f)
                {
                    SetAlpha(1f);
                    transform.localScale = Vector3.one;
                    _phase = Phase.Idle;
                }
            }
            else if (_phase == Phase.Death)
            {
                float s = 1f - t;
                transform.localScale = new Vector3(s, s, 1f);
                if (t >= 1f)
                {
                    transform.localScale = Vector3.zero;
                    SetAlpha(0f);
                    _phase = Phase.Idle;
                }
            }
        }

        private void SnapTo(bool alive)
        {
            _phase = Phase.Idle;
            _elapsed = 0f;
            SetAlpha(alive ? 1f : 0f);
            transform.localScale = Vector3.one;
        }

        private void SetAlpha(float a)
        {
            var c = spriteRenderer.color;
            c.a = a;
            spriteRenderer.color = c;
        }
    }
}
