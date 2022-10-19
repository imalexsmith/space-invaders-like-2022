using UnityEngine;
using Cinemachine;
using NightFramework;


[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : PrimitivePoolable, ILevelBoundExitReaction, IDamageableHitReaction
{
    // ====================================================
    public float Damage = 1f;
    public float MoveSpeed = 10f;

    [Space]
    public float HitImpulseForce = 0.1f;
    public CinemachineImpulseSource HitImpulse;

    [Space]
    public PoolableVFX ImpactVFXPrefab;

    [Space]
    public Rigidbody2D CachedRigidbody2D;


    // ====================================================
    public void OnLevelBoundTriggerExit(LevelBound bound)
    {
        ReturnToPool();
    }

    public void OnDamageableHit(IDamageable damageable)
    {
        damageable.TakeDamage(Damage);

        if (HitImpulse)
        {
            HitImpulse.GenerateImpulse(-HitImpulseForce * transform.up);
        }

        if (ImpactVFXPrefab != null)
        {
            var vfx = MassSpawner2.Instance.Spawn(ImpactVFXPrefab);
            vfx.transform.position = transform.position;
        }

        ReturnToPool();
    }

    protected void Reset()
    {
        CachedRigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected void Awake()
    {
        if (!CachedRigidbody2D)
            CachedRigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected void FixedUpdate()
    {
        var pos = CachedRigidbody2D.position + MoveSpeed * Time.deltaTime * (Vector2)transform.up;
        CachedRigidbody2D.MovePosition(pos);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody.TryGetComponent<IDamageable>(out var target))
            OnDamageableHit(target);
    }
}