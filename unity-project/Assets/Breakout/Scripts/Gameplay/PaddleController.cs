using UnityEngine;
using UnityEngine.InputSystem;

namespace BreakoutGameplay
{
    public sealed class PaddleController : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference pointerAction;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float paddleHalfWidth = 1f;

        private Camera mainCam;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        private void OnEnable()
        {
            moveAction?.action.Enable();
            pointerAction?.action.Enable();
        }

        private void OnDisable()
        {
            moveAction?.action.Disable();
            pointerAction?.action.Disable();
        }

        private void Update()
        {
            if (moveAction != null)
            {
                float input = moveAction.action.ReadValue<float>();
                float x = transform.position.x + input * moveSpeed * Time.deltaTime;
                transform.position = new Vector3(BreakoutGame.Paddle.ClampX(x, paddleHalfWidth), transform.position.y, transform.position.z);
            }

            if (pointerAction != null && mainCam != null)
            {
                Vector2 screenPos = pointerAction.action.ReadValue<Vector2>();
                Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
                float x = BreakoutGame.Paddle.ClampX(worldPos.x, paddleHalfWidth);
                transform.position = new Vector3(x, transform.position.y, transform.position.z);
            }
        }
    }
}
