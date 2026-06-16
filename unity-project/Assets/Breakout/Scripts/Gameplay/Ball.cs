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
        public Vector2 Velocity => rb.velocity;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.drag = 0f;
            rb.angularDrag = 0f;
        }

        private void Start()
        {
            Launch();
        }

        public void Launch()
        {
            float angle = Random.Range(-launchAngleRange, launchAngleRange);
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
            rb.velocity = direction.normalized * speed;
        }

        public void SetVelocityForTest(Vector2 velocity)
        {
            rb.velocity = velocity;
        }

        private void FixedUpdate()
        {
            if (rb.velocity.sqrMagnitude > 0.0001f)
            {
                rb.velocity = rb.velocity.normalized * speed;
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
                rb.velocity = ReflectionHelper.PaddleBounce(rb.velocity, normalizedOffset);
            }
            else if (Mathf.Abs(normal.x) > 0.5f)
            {
                rb.velocity = ReflectionHelper.ReflectVerticalWall(rb.velocity);
            }
            else if (Mathf.Abs(normal.y) > 0.5f)
            {
                rb.velocity = ReflectionHelper.ReflectHorizontalWall(rb.velocity);
            }
        }
    }
}
