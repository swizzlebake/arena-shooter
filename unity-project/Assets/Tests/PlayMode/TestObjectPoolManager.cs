using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Game;
using Gameplay;

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
            managerGO.SetActive(false);
            poolManager = managerGO.AddComponent<ObjectPoolManager>();

            var enemyField = typeof(ObjectPoolManager).GetField("enemyPrefab",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var bulletField = typeof(ObjectPoolManager).GetField("bulletPrefab",
                BindingFlags.NonPublic | BindingFlags.Instance);

            enemyField?.SetValue(poolManager, enemyPrefab);
            bulletField?.SetValue(poolManager, bulletPrefab);

            managerGO.SetActive(true);

            typeof(ObjectPoolManager).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(poolManager, null);
        }

        [TearDown]
        public void TearDown()
        {
            var enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            foreach (var e in enemies) Object.DestroyImmediate(e.gameObject);

            var bullets = Object.FindObjectsByType<Bullet>(FindObjectsSortMode.None);
            foreach (var b in bullets) Object.DestroyImmediate(b.gameObject);

            if (poolManager != null)
            {
                var enemyPoolField = typeof(ObjectPoolManager).GetField("enemyPool",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                var bulletPoolField = typeof(ObjectPoolManager).GetField("bulletPool",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                var enemyPool = enemyPoolField?.GetValue(poolManager) as ObjectPool;
                var bulletPool = bulletPoolField?.GetValue(poolManager) as ObjectPool;

                enemyPool?.ReturnAll();
                bulletPool?.ReturnAll();
            }

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
            Assert.IsTrue(enemies.Length > 0, "No enemies found in scene");
            bool foundPrimed = false;
            foreach (var e in enemies)
            {
                if (e.gameObject.GetInstanceID() == primedId) foundPrimed = true;
            }
            Assert.IsTrue(foundPrimed, "Primed enemy instance not found among spawned enemies");

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
            Assert.IsTrue(bullets.Length > 0, "No bullets found in scene");
            bool foundPrimed = false;
            foreach (var b in bullets)
            {
                if (b.gameObject.GetInstanceID() == primedId) foundPrimed = true;
            }
            Assert.IsTrue(foundPrimed, "Primed bullet instance not found among spawned bullets");

            Object.DestroyImmediate(playerGO);
        }
    }
}
