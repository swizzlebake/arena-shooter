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

        public Vector2 AimDirection { get; private set; }

        private Rigidbody2D rb;

        public void Configure(InputActionReference move, InputActionReference aim)
        {
            moveAction = move;
            aimAction = aim;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
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
    }
}
