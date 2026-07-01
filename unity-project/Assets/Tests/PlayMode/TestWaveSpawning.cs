using NUnit.Framework;
using UnityEngine;
using Game;

namespace Gameplay.Tests
{
    [TestFixture]
    public class TestWaveSpawning : InputTestFixture
    {
        private GameObject spawnerGO;
        private EnemySpawner spawner;
        private WavePlan wavePlan;

        [SetUp]
        public void SetUp()
        {
            spawnerGO = new GameObject("EnemySpawner");
            spawner = spawnerGO.AddComponent<EnemySpawner>();

            var wave1 = new WaveConfig(0.5f, 3, 1f);
            var wave2 = new WaveConfig(0.3f, 5, 1.5f);
            wavePlan = WavePlan.Create(wave1, wave2);

            spawner.Configure(wavePlan);
        }

        [TearDown]
        public void TearDown()
        {
            if (spawnerGO != null)
            {
                Object.DestroyImmediate(spawnerGO);
            }
        }

        [Test]
        public void Spawner_StartsAtWaveZero()
        {
            spawner.StartSpawning();
            Assert.AreEqual(0, spawner.CurrentWave);
        }

        [Test]
        public void Spawner_IsSpawning_BecomesTrue()
        {
            Assert.IsFalse(spawner.IsSpawning);
            spawner.StartSpawning();
            Assert.IsTrue(spawner.IsSpawning);
        }

        [Test]
        public void Spawner_StopSpawning_Stops()
        {
            spawner.StartSpawning();
            spawner.StopSpawning();
            Assert.IsFalse(spawner.IsSpawning);
        }

        [Test]
        public void Spawner_OnEnemyKilled_DecrementsCount()
        {
            spawner.StartSpawning();
            spawner.SimulateSpawn();
            Assert.AreEqual(1, spawner.CurrentEnemyCount);

            spawner.OnEnemyKilled();
            Assert.AreEqual(0, spawner.CurrentEnemyCount);
        }

        [Test]
        public void Spawner_OnEnemyKilled_DoesNotGoNegative()
        {
            spawner.StartSpawning();
            Assert.AreEqual(0, spawner.CurrentEnemyCount);

            spawner.OnEnemyKilled();
            Assert.AreEqual(0, spawner.CurrentEnemyCount);
        }

        [Test]
        public void Spawner_RespectsMaxEnemiesCap()
        {
            spawner.Configure(null, 0.1f, 2);
            spawner.StartSpawning();

            for (int i = 0; i < 5; i++)
            {
                spawner.SimulateSpawn();
            }

            Assert.LessOrEqual(spawner.CurrentEnemyCount, 2);
        }
    }
}
