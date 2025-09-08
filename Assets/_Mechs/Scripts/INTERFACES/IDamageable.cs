// IDamageable.cs
public interface IDamageable
{
    void TakeDamage(DamageInfo info);
    bool IsAlive { get; }
}
