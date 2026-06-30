using UnityEngine;

namespace Gameplay
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float fireRate = 0.2f;
        [SerializeField] private float bulletDamage = 1f;

        private PlayerController playerController;
        private float timer;

        public float FireRate => fireRate;
        public float CurrentTimer => timer;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= fireRate)
            {
                timer = 0f;

                if (playerController != null && playerController.AimDirection.sqrMagnitude > 0f)
                {
                    float angle = Mathf.Atan2(playerController.AimDirection.y, playerController.AimDirection.x) * Mathf.Rad2Deg;
                    var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0f, 0f, angle));
                    if (bullet != null)
                    {
                        var bulletScript = bullet.GetComponent<Bullet>();
                        if (bulletScript != null)
                        {
                            bulletScript.Initialize(playerController.AimDirection, 0f, bulletDamage);
                        }
                    }
                }
            }
        }

        public Quaternion GetBulletRotation(Vector2 aimDir)
        {
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0f, 0f, angle);
        }

        public void AdvanceTimer(float dt) => timer += dt;
    }
}
