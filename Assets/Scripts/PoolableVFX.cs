using UnityEngine;
using NightFramework;


[RequireComponent(typeof(ParticleSystem))]
public class PoolableVFX : PrimitivePoolable
{
    // ====================================================
    public ParticleSystem CachedParticleSystem;


    // ====================================================
    public override void WakeUpPoolable(bool riseEvents = true)
    {
        base.WakeUpPoolable(riseEvents);

        if (riseEvents)
            CachedParticleSystem.Play();
    }

    protected void Reset()
    {
        CachedParticleSystem = GetComponent<ParticleSystem>();
        var main = CachedParticleSystem.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    protected void Awake()
    {
        if (!CachedParticleSystem)
            CachedParticleSystem = GetComponent<ParticleSystem>();
    }

    protected void OnParticleSystemStopped()
    {
        ReturnToPool();
    }
}