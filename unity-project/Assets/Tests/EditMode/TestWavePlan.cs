using NUnit.Framework;
using Game;

namespace Game.Tests
{
    [TestFixture]
    public class TestWavePlan
    {
        private WaveConfig wave1;
        private WaveConfig wave2;
        private WavePlan plan;

        [SetUp]
        public void SetUp()
        {
            wave1 = new WaveConfig(2f, 5, 3f);
            wave2 = new WaveConfig(1f, 8, 4f);
            plan = WavePlan.Create(wave1, wave2);
        }

        [Test]
        public void WaveConfig_StoresValuesCorrectly()
        {
            Assert.AreEqual(2f, wave1.SpawnInterval);
            Assert.AreEqual(5, wave1.EnemiesToSpawn);
            Assert.AreEqual(3f, wave1.InterWaveDelay);
        }

        [Test]
        public void WavePlan_ReturnsCorrectWaveCount()
        {
            Assert.AreEqual(2, plan.CurrentWaveCount);
        }

        [Test]
        public void WavePlan_Indexer_ReturnsCorrectWave()
        {
            Assert.AreEqual(wave1, plan[0]);
            Assert.AreEqual(wave2, plan[1]);
        }

        [Test]
        public void WavePlan_GetWave_ReturnsCorrectWave()
        {
            Assert.AreEqual(wave1, plan.GetWave(0));
            Assert.AreEqual(wave2, plan.GetWave(1));
        }

        [Test]
        public void WavePlan_HasNextWave_ReturnsTrueForFirstWave()
        {
            Assert.IsTrue(plan.HasNextWave(0));
        }

        [Test]
        public void WavePlan_HasNextWave_ReturnsFalseForLastWave()
        {
            Assert.IsFalse(plan.HasNextWave(1));
        }

        [Test]
        public void WavePlan_EscalatingSpawnIntervals()
        {
            Assert.Less(wave2.SpawnInterval, wave1.SpawnInterval);
        }

        [Test]
        public void WavePlan_EscalatingEnemyCounts()
        {
            Assert.Greater(wave2.EnemiesToSpawn, wave1.EnemiesToSpawn);
        }

        [Test]
        public void WavePlan_CreateWithEmptyList()
        {
            var emptyPlan = WavePlan.Create(new WaveConfig[0]);
            Assert.AreEqual(0, emptyPlan.CurrentWaveCount);
            Assert.IsFalse(emptyPlan.HasNextWave(-1));
        }

        [Test]
        public void WavePlan_WavesProperty_ReturnsReadOnlyList()
        {
            Assert.IsNotNull(plan.Waves);
            Assert.AreEqual(2, plan.Waves.Count);
        }
    }
}
