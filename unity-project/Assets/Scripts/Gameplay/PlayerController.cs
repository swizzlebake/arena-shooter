using UnityEngine;

namespace Gameplay
{
    public class PlayerController : MonoBehaviour, Game.IDamageable
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float maxHealth = 10f;
        [SerializeField] private GameObject hitFlashPrefab;
        [SerializeField] private GameObject deathBurstPrefab;

        public Vector2 AimDirection { get; private set; }
        public bool IsAlive { get; private set; } = true;

        private Rigidbody2D rb;
        private GameManager gameManager;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            gameManager = Object.FindFirstObjectByType<GameManager>();
        }

        private void Update()
        {
            if (!IsAlive) return;

            Vector2 moveInput = new Vector2(
                UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.w].isPressed ? 1f : 0f - (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.s].isPressed ? 1f : 0f),
                UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.a].isPressed ? 1f : 0f - (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.d].isPressed ? 1f : 0f)
            );

            if (moveInput.sqrMagnitude > 0f)
            {
                Vector2 targetPos = rb.position + moveInput.normalized * moveSpeed * Time.deltaTime;
                rb.MovePosition(targetPos);
            }

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            AimDirection = (mousePos - (Vector2)transform.position).normalized;
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive) return;

            maxHealth -= amount;

            if (hitFlashPrefab != null)
            {
                Object.Instantiate(hitFlashPrefab, transform.position, Quaternion.identity);
            }

            AudioManager.Instance?.PlayHitSFX();

            if (maxHealth <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            IsAlive = false;

            if (deathBurstPrefab != null)
            {
                Object.Instantiate(deathBurstPrefab, transform.position, Quaternion.identity);
            }

            AudioManager.Instance?.PlayDeathSFX();

            if (gameManager != null)
            {
                gameManager.OnPlayerDied();
            }
        }

        public void Configure(float maxHealth, float moveSpeed)
        {
            this.maxHealth = maxHealth;
            this.moveSpeed = moveSpeed;
            IsAlive = true;
        }
    }
}
