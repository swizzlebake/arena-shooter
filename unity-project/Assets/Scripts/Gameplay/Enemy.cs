using UnityEngine;

namespace Gameplay
{
    public class Enemy : MonoBehaviour, Game.IDamageable
    {
        [SerializeField] private float maxHealth = 2f;
        [SerializeField] private float moveSpeed = 3f;

        public bool IsAlive { get; private set; } = true;

        private Rigidbody2D rb;
        private PlayerController playerController;

        public Vector2 MoveDirection =>
            playerController != null ? (playerController.transform.position - transform.position).normalized : Vector2.zero;

        public void Configure(float maxHealth, float moveSpeed)
        {
            this.maxHealth = maxHealth;
            this.moveSpeed = moveSpeed;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerController = UnityEngine.Object.FindFirstObjectByType<PlayerController>();
        }

        private void Update()
        {
            if (!IsAlive || playerController == null) return;

            Vector2 direction = MoveDirection;
            if (direction.sqrMagnitude > 0f)
            {
                Vector2 targetPos = rb.position + direction * moveSpeed * Time.deltaTime;
                rb.MovePosition(targetPos);
            }
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive) return;

            maxHealth -= amount;
            if (maxHealth <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            IsAlive = false;
            Destroy(gameObject);
        }
    }
}
