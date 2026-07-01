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
            enemyPool = new ObjectPool(enemyPrefab, initialEnemyCount);
            bulletPool = new ObjectPool(bulletPrefab, initialBulletCount);
        }

        private void Start()
        {
            enemyPool.PreWarm();
            bulletPool.PreWarm();
        }

        public GameObject CheckoutEnemy(Vector2 position)
        {
            return enemyPool.Checkout(position, Quaternion.identity);
        }

        public void ReturnEnemy(GameObject obj)
        {
            enemyPool.Return(obj);
        }

        public GameObject CheckoutBullet(Vector2 position, Quaternion rotation)
        {
            return bulletPool.Checkout(position, rotation);
        }

        public void ReturnBullet(GameObject obj)
        {
            bulletPool.Return(obj);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
