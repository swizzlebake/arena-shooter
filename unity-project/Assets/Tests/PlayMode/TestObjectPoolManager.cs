using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Game;

namespace Gameplay.Tests
{
    public class TestObjectPoolManager
    {
        private GameObject managerGO;
        private ObjectPoolManager poolManager;
        private GameObject enemyPrefab;
        private GameObject bulletPrefab;

        [SetUp]
        public void SetUp()
        {
            enemyPrefab = new GameObject("EnemyPrefab");
            enemyPrefab.AddComponent<Rigidbody2D>().gravityScale = 0f;
            enemyPrefab.AddComponent<BoxCollider2D>();
            enemyPrefab.AddComponent<Enemy>();

            bulletPrefab = new GameObject("BulletPrefab");
            bulletPrefab.AddComponent<Rigidbody2D>().gravityScale = 0f;
            bulletPrefab.AddComponent<BoxCollider2D>();
            bulletPrefab.AddComponent<Bullet>();

            managerGO = new GameObject("PoolManager");
            poolManager = managerGO.AddComponent<ObjectPoolManager>();

            var enemyField = typeof(ObjectPoolManager).GetField("enemyPrefab",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var bulletField = typeof(ObjectPoolManager).GetField("bulletPrefab",
                BindingFlags.NonPublic | BindingFlags.Instance);

            enemyField?.SetValue(poolManager, enemyPrefab);
            bulletField?.SetValue(poolManager, bulletPrefab);

            poolManager.Awake();
            poolManager.Start();
        }

        [TearDown]
        public void TearDown()
        {
            if (managerGO != null)
                Object.DestroyImmediate(managerGO);
            Object.DestroyImmediate(enemyPrefab);
            Object.DestroyImmediate(bulletPrefab);
        }

        [Test]
        public void Pool_CheckoutEnemy_ReturnsActiveObject()
        {
            var enemy = poolManager.CheckoutEnemy(Vector2.zero);
            Assert.IsNotNull(enemy);
            Assert.IsTrue(enemy.activeSelf);
        }

        [Test]
        public void Pool_ReturnEnemy_DeactivatesObject()
        {
            var enemy = poolManager.CheckoutEnemy(Vector2.zero);
            poolManager.ReturnEnemy(enemy);
            Assert.IsFalse(enemy.activeSelf);
        }

        [Test]
        public void Pool_CheckoutBullet_ReturnsActiveObject()
        {
            var bullet = poolManager.CheckoutBullet(Vector2.zero, Quaternion.identity);
            Assert.IsNotNull(bullet);
            Assert.IsTrue(bullet.activeSelf);
        }

        [Test]
        public void Pool_ReturnBullet_DeactivatesObject()
        {
            var bullet = poolManager.CheckoutBullet(Vector2.zero, Quaternion.identity);
            poolManager.ReturnBullet(bullet);
            Assert.IsFalse(bullet.activeSelf);
        }

        [Test]
        public void Pool_ReusesEnemyInstances()
        {
            var e1 = poolManager.CheckoutEnemy(Vector2.zero);
            int id = e1.GetInstanceID();
            poolManager.ReturnEnemy(e1);

            var e2 = poolManager.CheckoutEnemy(Vector2.zero);
            Assert.AreEqual(id, e2.GetInstanceID());
        }

        [Test]
        public void Pool_ReusesBulletInstances()
        {
            var b1 = poolManager.CheckoutBullet(Vector2.zero, Quaternion.identity);
            int id = b1.GetInstanceID();
            poolManager.ReturnBullet(b1);

            var b2 = poolManager.CheckoutBullet(Vector2.zero, Quaternion.identity);
            Assert.AreEqual(id, b2.GetInstanceID());
        }

        [Test]
        public void EnemySpawner_PullsFromPool()
        {
            var primed = poolManager.CheckoutEnemy(Vector2.zero);
            int primedId = primed.GetInstanceID();
            poolManager.ReturnEnemy(primed);

            var spawnerGO = new GameObject("Spawner");
            var spawner = spawnerGO.AddComponent<EnemySpawner>();

            var waveConfig = new WaveConfig(1f, 5, 2f);
            spawner.Configure(WavePlan.Create(waveConfig));

            spawner.StartSpawning();
            spawner.SimulateSpawn();

            var enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            Assert.AreEqual(1, enemies.Length);
            Assert.AreEqual(primedId, enemies[0].gameObject.GetInstanceID());

            Object.DestroyImmediate(spawnerGO);
        }

        [Test]
        public void Weapon_PullsFromPool()
        {
            var primed = poolManager.CheckoutBullet(Vector2.zero, Quaternion.identity);
            int primedId = primed.GetInstanceID();
            poolManager.ReturnBullet(primed);

            var playerGO = new GameObject("Player");
            playerGO.AddComponent<Rigidbody2D>().gravityScale = 0f;
            var controller = playerGO.AddComponent<PlayerController>();
            var weapon = playerGO.AddComponent<Weapon>();

            var aimField = typeof(PlayerController).GetField("aimDirection",
                BindingFlags.NonPublic | BindingFlags.Instance);
            aimField?.SetValue(controller, Vector2.right);

            weapon.AdvanceTimer(1f);

            typeof(Weapon).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(weapon, null);

            var bullets = Object.FindObjectsByType<Bullet>(FindObjectsSortMode.None);
            Assert.AreEqual(1, bullets.Length);
            Assert.AreEqual(primedId, bullets[0].gameObject.GetInstanceID());

            Object.DestroyImmediate(playerGO);
        }
    }
}
