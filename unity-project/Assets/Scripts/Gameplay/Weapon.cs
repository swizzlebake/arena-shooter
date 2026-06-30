using UnityEngine;

namespace Gameplay
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float fireRate = 0.2f;

        private PlayerController playerController;
        private float timer;

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
                    var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                    if (bullet != null)
                    {
                        var bulletScript = bullet.GetComponent<Bullet>();
                        if (bulletScript != null)
                        {
                            bulletScript.Initialize(playerController.AimDirection, 0f);
                        }
                    }
                }
            }
        }
    }
}
