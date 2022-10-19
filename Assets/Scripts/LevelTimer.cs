using UnityEngine;
using UltEvents;


public class LevelTimer : MonoBehaviour
{
    // ====================================================
    public UltEvent OnTimeIsUp = new();

    [Space]
    public float DefaultTime = 150f;

    private float _remainingTime;
    public float RemainingTime
    {
        get => _remainingTime;
        private set
        {
            if (_remainingTime != value)
            {
                _remainingTime = value;
                if (_remainingTime <= 0f)
                {
                    _remainingTime = 0f;
                    IsTimeUp = true;
                }
            }
        }
    }

    private bool _isTimeUp;
    public bool IsTimeUp 
    {
        get => _isTimeUp;
        private set
        {
            if (_isTimeUp != value)
            {
                _isTimeUp = value;
                if (_isTimeUp)
                {
                    StopTimer();
                    LevelManager.Instance.FinishLevel(false);
                    OnTimeIsUp.Invoke();
                }
            }
        }
    }

    public bool IsStarted { get; private set; }



    // ====================================================
    protected void Awake()
    {
        RemainingTime = DefaultTime;
        IsStarted = true;
    }

    protected void Start()
    {
        LevelManager.Instance.OnVictory += StopTimer;
    }

    protected void Update()
    {
        if (IsStarted)
            RemainingTime -= Time.deltaTime;
    }

    protected void OnDestroy()
    {
        if (LevelManager.IsReady)
            LevelManager.Instance.OnVictory -= StopTimer;
    }

    private void StopTimer()
    {
        IsStarted = false;
    }
}