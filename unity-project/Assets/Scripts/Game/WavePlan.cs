using System.Collections.Generic;

namespace Game
{
    [System.Serializable]
    public class WavePlan
    {
        private readonly IReadOnlyList<WaveConfig> waves;

        public IReadOnlyList<WaveConfig> Waves => waves;
        public WaveConfig this[int index] => waves[index];
        public int CurrentWaveCount => waves.Count;

        public bool HasNextWave(int currentIndex) => currentIndex + 1 < waves.Count;

        public WaveConfig GetWave(int index) => waves[index];

        private WavePlan(IReadOnlyList<WaveConfig> waves)
        {
            this.waves = waves;
        }

        public static WavePlan Create(params WaveConfig[] waves) => new WavePlan(waves);

        public static WavePlan Create(IList<WaveConfig> waves) => new WavePlan(waves);
    }
}
