using UnityEngine;

namespace Gameplay
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float bulletSpeed = 12f;
        [SerializeField] private float lifetime = 3f;

        private Rigidbody2D rb;
        private float timer;

        public void Initialize(Vector2 direction, float speed)
        {
            rb.linearVelocity = direction * (speed > 0f ? speed : bulletSpeed);
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(1f);
                Destroy(gameObject);
            }
        }
    }
}
