using NUnit.Framework;
using UnityEngine;

namespace Gameplay.Tests
{
    public class TestEnemyAndSpawner
    {
        private GameObject playerGO;
        private PlayerController playerController;

        [SetUp]
        public void SetUp()
        {
            playerGO = new GameObject("Player");
            var rb = playerGO.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            playerController = playerGO.AddComponent<PlayerController>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(playerGO);
        }

        [Test]
        public void Enemy_TakeDamage_DestroysWhenHealthExhausted()
        {
            var enemyGO = new GameObject("Enemy");
            enemyGO.AddComponent<Rigidbody2D>().gravityScale = 0f;
            var enemy = enemyGO.AddComponent<Enemy>();

            enemy.Configure(2f, 3f);
            Assert.IsTrue(enemy.IsAlive);

            enemy.TakeDamage(1f);
            Assert.IsTrue(enemy.IsAlive);

            enemy.TakeDamage(2f);
            Assert.IsFalse(enemy.IsAlive);

            Object.DestroyImmediate(enemyGO);
        }

        [Test]
        public void Enemy_MoveDirection_PointsToPlayer()
        {
            var enemyGO = new GameObject("Enemy");
            enemyGO.AddComponent<Rigidbody2D>().gravityScale = 0f;
            var enemy = enemyGO.AddComponent<Enemy>();

            playerController.transform.position = new Vector3(5f, 0f, 0f);
            enemy.transform.position = new Vector3(0f, 0f, 0f);

            var dir = enemy.MoveDirection;
            Assert.AreEqual(Vector3.right, (Vector3)dir);

            Object.DestroyImmediate(enemyGO);
        }

        [Test]
        public void EnemySpawner_StartStopSpawning_TogglesState()
        {
            var spawnerGO = new GameObject("Spawner");
            var dummyPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.DontDestroyOnLoad(dummyPrefab);

            var spawner = spawnerGO.AddComponent<EnemySpawner>();
            spawner.Configure(dummyPrefab, 1f, 5);

            Assert.IsFalse(spawner.IsSpawning);
            
            spawner.StartSpawning();
            Assert.IsTrue(spawner.IsSpawning);

            spawner.StopSpawning();
            Assert.IsFalse(spawner.IsSpawning);

            Object.DestroyImmediate(spawnerGO);
            Object.DestroyImmediate(dummyPrefab);
        }

        [Test]
        public void EnemySpawner_SpawnCap_EnforcesMaxEnemies()
        {
            var spawnerGO = new GameObject("Spawner");
            var dummyPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.DontDestroyOnLoad(dummyPrefab);

            var spawner = spawnerGO.AddComponent<EnemySpawner>();
            spawner.Configure(dummyPrefab, 1f, 3);

            for (int i = 0; i < 5; i++)
            {
                spawner.SimulateSpawn();
            }

            Assert.AreEqual(3, spawner.CurrentEnemyCount);

            Object.DestroyImmediate(spawnerGO);
            Object.DestroyImmediate(dummyPrefab);
        }
    }
}