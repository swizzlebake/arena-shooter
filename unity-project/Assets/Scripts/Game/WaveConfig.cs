namespace Game
{
    public readonly struct WaveConfig
    {
        public float SpawnInterval { get; }
        public int EnemiesToSpawn { get; }
        public float InterWaveDelay { get; }

        public WaveConfig(float spawnInterval, int enemiesToSpawn, float interWaveDelay)
        {
            SpawnInterval = spawnInterval;
            EnemiesToSpawn = enemiesToSpawn;
            InterWaveDelay = interWaveDelay;
        }
    }
}
