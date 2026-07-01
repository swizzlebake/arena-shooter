using Game;
using UnityEngine;

namespace Gameplay
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float bulletSpeed = 12f;
        [SerializeField] private float lifetime = 3f;

        private Rigidbody2D rb;
        private float timer;
        private float damage;

        public void Initialize(Vector2 direction, float speed, float bulletDamage)
        {
            damage = bulletDamage;
            timer = 0f;
            rb.linearVelocity = direction * (speed > 0f ? speed : bulletSpeed);
        }

        public void SetDamage(float amount) => damage = amount;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= lifetime)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            if (ObjectPoolManager.Instance != null)
            {
                ObjectPoolManager.Instance.ReturnBullet(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SimulateTriggerEnter(Collider2D other) => OnTriggerEnter2D(other);
    }
}
