using UnityEngine;
using BreakoutGame;

namespace BreakoutGameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public sealed class Ball : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float launchAngleRange = 15f;

        private Rigidbody2D rb;

        public float Speed => speed;
        public Vector2 Velocity => rb.linearVelocity;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
        }

        private void Start()
        {
            Launch();
        }

        public void Launch()
        {
            float angle = Random.Range(-launchAngleRange, launchAngleRange);
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
            rb.linearVelocity = direction.normalized * speed;
        }

        public void Configure(float speed)
        {
            this.speed = speed;
        }

        public void SetVelocityForTest(Vector2 velocity)
        {
            rb.linearVelocity = velocity;
        }

        private void FixedUpdate()
        {
            if (rb.linearVelocity.sqrMagnitude > 0.0001f)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * speed;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Vector2 normal = collision.contacts[0].normal;

            if (collision.gameObject.CompareTag("Paddle"))
            {
                float paddleHalfWidth = collision.collider.bounds.extents.x;
                Vector2 contactPoint = collision.GetContact(0).point;
                float paddleCenterX = collision.collider.bounds.center.x;
                float offset = contactPoint.x - paddleCenterX;
                float normalizedOffset = Mathf.Clamp(offset / paddleHalfWidth, -1f, 1f);
                rb.linearVelocity = ReflectionHelper.PaddleBounce(rb.linearVelocity, normalizedOffset);
            }
            else if (Mathf.Abs(normal.x) > 0.5f)
            {
                rb.linearVelocity = ReflectionHelper.ReflectVerticalWall(rb.linearVelocity);
            }
            else if (Mathf.Abs(normal.y) > 0.5f)
            {
                rb.linearVelocity = ReflectionHelper.ReflectHorizontalWall(rb.linearVelocity);
            }
        }
    }
}
