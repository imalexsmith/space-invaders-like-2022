using UnityEngine;
using UnityEngine.InputSystem;
using NightFramework;


[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    // ====================================================
    public float ShootDelay = 0.5f;

    [Space]
    public Transform FirePoint;
    public AudioSource FireSFX;

    [Space]
    public Bullet BulletPrefab;

    [Space]
    public PlayerInput CachedPlayerInput;

    private bool _isShooting;
    private float _nextShootTime;
    private AimCursor _aimCursor;


    // ====================================================
    protected void Reset()
    {
        CachedPlayerInput = GetComponent<PlayerInput>();
    }

    protected void Awake()
    {
        if (!CachedPlayerInput)
            CachedPlayerInput = GetComponent<PlayerInput>();
    }

    protected void Start()
    {
        _aimCursor = FindObjectOfType<AimCursor>();
        _aimCursor.OnPositionChanged += AimTo;

        LevelManager.Instance.OnVictory += DeactivateInput;
        LevelManager.Instance.OnDefeat += DeactivateInput;

        _nextShootTime = Time.time + ShootDelay;
    }

    protected void Update()
    {
        if (_isShooting && Time.time >= _nextShootTime)
            Fire();
    }

    protected void OnDestroy()
    {
        if (_aimCursor)
            _aimCursor.OnPositionChanged -= AimTo;

        if (LevelManager.IsReady)
        {
            LevelManager.Instance.OnVictory -= DeactivateInput;
            LevelManager.Instance.OnDefeat -= DeactivateInput;
        }
    }

    private void AimTo(Vector3 worldPos)
    {
        var dir = worldPos - transform.position;
        var cosA = dir.x / dir.magnitude;
        var a = Mathf.Acos(cosA) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, a - 90f);
    }

    private void Fire()
    {
        var bullet = MassSpawner2.Instance.Spawn(BulletPrefab);
        bullet.transform.SetPositionAndRotation(FirePoint.position, transform.rotation);

        _nextShootTime = Time.time + ShootDelay;

        if (FireSFX)
            FireSFX.Play();
    }

    private void DeactivateInput()
    {
        _isShooting = false;
        CachedPlayerInput.DeactivateInput();
    }

    private void OnFire(InputValue value)
    {
        _isShooting = value.isPressed;
    }
}