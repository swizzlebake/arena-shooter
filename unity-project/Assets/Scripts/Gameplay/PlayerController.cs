using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        private const float MovementThreshold = 0.01f;

        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference aimAction;
        [SerializeField] private float moveSpeed = 5f;

        [SerializeField] private GameManager gameManager;
        [SerializeField] private float maxHealth = 3f;

        public Vector2 AimDirection { get; private set; }
        private float currentHealth;

        private Rigidbody2D rb;

        public void Configure(InputActionReference move, InputActionReference aim)
        {
            moveAction = move;
            aimAction = aim;
        }

        public void Configure(GameManager manager) => gameManager = manager;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            currentHealth = maxHealth;
        }

        private void Update()
        {
            if (moveAction?.action != null)
            {
                Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
                if (moveInput.sqrMagnitude > MovementThreshold)
                {
                    Vector2 targetPos = rb.position + moveInput * moveSpeed * Time.deltaTime;
                    rb.MovePosition(Game.ArenaBounds.Default.Clamp(targetPos));
                }
            }

            if (aimAction?.action != null)
            {
                Vector2 aimInput = aimAction.action.ReadValue<Vector2>();
                if (aimInput.sqrMagnitude > MovementThreshold)
                {
                    AimDirection = aimInput.normalized;
                    float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle);
                }
            }
        }

        public void TakeDamage(float amount)
        {
            if (currentHealth <= 0f) return;

            currentHealth -= amount;
            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            if (gameManager != null)
            {
                gameManager.TriggerGameOver();
            }
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null && enemy.IsAlive)
            {
                TakeDamage(1f);
            }
        }
    }
}
