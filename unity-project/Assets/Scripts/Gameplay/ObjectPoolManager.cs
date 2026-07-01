using UnityEngine;

namespace Gameplay
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager Instance { get; private set; }

        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int initialEnemyCount = 10;
        [SerializeField] private int initialBulletCount = 50;

        private ObjectPool enemyPool;
        private ObjectPool bulletPool;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (enemyPrefab != null)
            {
                enemyPool = new ObjectPool(enemyPrefab, initialEnemyCount);
            }
            else
            {
                Debug.LogWarning("ObjectPoolManager: enemyPrefab is not assigned. Enemy pool will not be created.");
            }

            if (bulletPrefab != null)
            {
                bulletPool = new ObjectPool(bulletPrefab, initialBulletCount);
            }
            else
            {
                Debug.LogWarning("ObjectPoolManager: bulletPrefab is not assigned. Bullet pool will not be created.");
            }
        }

        private void Start()
        {
            if (enemyPool != null)
                enemyPool.PreWarm();
            if (bulletPool != null)
                bulletPool.PreWarm();
        }

        public GameObject CheckoutEnemy(Vector2 position)
        {
            if (enemyPool == null) return null;
            return enemyPool.Checkout(position, Quaternion.identity);
        }

        public void ReturnEnemy(GameObject obj)
        {
            if (enemyPool != null)
                enemyPool.Return(obj);
        }

        public GameObject CheckoutBullet(Vector2 position, Quaternion rotation)
        {
            if (bulletPool == null) return null;
            return bulletPool.Checkout(position, rotation);
        }

        public void ReturnBullet(GameObject obj)
        {
            if (bulletPool != null)
                bulletPool.Return(obj);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
