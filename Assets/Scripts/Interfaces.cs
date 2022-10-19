public interface ILevelBoundEnterReaction
{
    public void OnLevelBoundTriggerEnter(LevelBound bound);
}

public interface ILevelBoundExitReaction
{
    public void OnLevelBoundTriggerExit(LevelBound bound);
}

public interface IDamageableHitReaction
{
    public void OnDamageableHit(IDamageable damageable);
}

public interface IDamageable
{
    public float CurrentHealth { get; }
    public bool IsKilled { get; }

    public void TakeDamage(float amount);
    public void Kill();
}