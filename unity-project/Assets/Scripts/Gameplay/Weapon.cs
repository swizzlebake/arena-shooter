using System;
using UnityEngine;

namespace Gameplay
{
    public class Weapon : MonoBehaviour
    {
        [Obsolete("Managed by ObjectPoolManager. Remove this field from the Inspector.", false)]
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
                    Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

                    GameObject bulletObj;
                    if (ObjectPoolManager.Instance != null)
                    {
                        bulletObj = ObjectPoolManager.Instance.CheckoutBullet(transform.position, rotation);
                    }
                    else
                    {
                        bulletObj = Instantiate(bulletPrefab, transform.position, rotation);
                    }

                    if (bulletObj != null)
                    {
                        var bulletScript = bulletObj.GetComponent<Bullet>();
                        if (bulletScript != null)
                        {
                            bulletScript.Initialize(playerController.AimDirection, 0f, bulletDamage);
                        }

                        AudioManager.Instance?.PlayFireSFX();
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
