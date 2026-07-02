namespace Game
{
    public interface IDamageable
    {
        void TakeDamage(float amount);
        bool IsAlive { get; }
    }
}
