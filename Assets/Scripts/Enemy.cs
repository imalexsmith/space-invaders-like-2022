using UnityEngine;
using UltEvents;
using NightFramework;


public class Enemy : PrimitivePoolable, ILevelBoundEnterReaction, ILevelBoundExitReaction, IDamageable
{
    // ====================================================
    public UltEvent<Enemy> OnKilled = new();

    [Space]
    public float DefaultHealth = 2f;
    public AnimationCurve SpeedX;
    public AnimationCurve SpeedY;

    [Space]
    public PoolableVFX ExplosionVFXPrefab;

    [Space]
    public Rigidbody2D CachedRigidbody2D;

    public float CurrentHealth { get; protected set; }
    public bool IsKilled { get; protected set; }

    private float _wakeUpTime;
    private float _moveSign = 1f;
    private bool _isOutOfBound;


    // ====================================================
    public void OnLevelBoundTriggerEnter(LevelBound bound)
    {
        InvertMovement();

        _isOutOfBound = true;
    }

    public void OnLevelBoundTriggerExit(LevelBound bound)
    {
        _isOutOfBound = false;
    }

    public void TakeDamage(float amount)
    {
        if (IsKilled)
            return;

        CurrentHealth -= amount;
        if (CurrentHealth <= 0f)
            Kill();
    }

    public void Kill()
    {
        CurrentHealth = 0f;
        IsKilled = true;

        if (ExplosionVFXPrefab != null)
        {
            var vfx = MassSpawner2.Instance.Spawn(ExplosionVFXPrefab);
            vfx.transform.position = transform.position;
        }

        ReturnToPool();
        OnKilled.Invoke(this);
    }

    public void InvertMovement()
    {
        _moveSign *= -1f;
    }

    public override void WakeUpPoolable(bool riseEvents = true)
    {
        base.WakeUpPoolable(riseEvents);

        CurrentHealth = DefaultHealth;
        IsKilled = false;
        _wakeUpTime = Time.time;
        _moveSign = 1f;
        _isOutOfBound = false;
    }

    protected void Reset()
    {
        CachedRigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected void Awake()
    {
        if (!CachedRigidbody2D)
            CachedRigidbody2D = GetComponent<Rigidbody2D>();

        SpeedX.postWrapMode = WrapMode.Loop;
        SpeedY.postWrapMode = WrapMode.Loop;
    }

    protected void FixedUpdate()
    {
        var t = Time.time - _wakeUpTime;
        var x = _moveSign * SpeedX.Evaluate(t);
        var y = _moveSign * SpeedY.Evaluate(t);

        if (_isOutOfBound)
        {
            if (transform.position.x > 0f)
                x = -Mathf.Abs(x);
            else if (transform.position.x < 0f)
                x = Mathf.Abs(x);

            if (transform.position.y > 0f)
                y = -Mathf.Abs(y);
            else if (transform.position.y < 0f)
                y = Mathf.Abs(y);
        }

        var speed = new Vector2(x, y);
        var pos = CachedRigidbody2D.position + Time.deltaTime * speed;

        CachedRigidbody2D.MovePosition(pos);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody.TryGetComponent<Enemy>(out _))
            InvertMovement();
    }

    protected void OnDrawGizmosSelected()
    {
        var max = Mathf.Max(SpeedX.keys[SpeedX.length - 1].time, SpeedY.keys[SpeedY.length - 1].time);
        var d = 0.1f;

        var pos = transform.position;
        for (float i = d; i < max; i += d)
        {
            var pos1 = pos + Time.fixedDeltaTime * new Vector3(SpeedX.Evaluate(i + d), SpeedY.Evaluate(i + d));
            Gizmos.DrawLine(pos, pos1);
            pos = pos1;
        }
    }
}