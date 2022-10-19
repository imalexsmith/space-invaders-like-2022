using System.Globalization;
using UnityEngine;
using TMPro;


public class LevelTimerView : MonoBehaviour
{
    // ====================================================
    public string TimeValueFormat = "0.00";
    public TMP_Text RemainingTimeText;

    private LevelTimer _levelTimer;
    private NumberFormatInfo _formatInfo = new() { NumberDecimalSeparator = ":" };


    // ====================================================
    protected void Start()
    {
        _levelTimer = FindObjectOfType<LevelTimer>();
        _levelTimer.OnTimeIsUp += TimeIsUp;
    }

    protected void LateUpdate()
    {
        if (_levelTimer.IsStarted)
            SetTimeText(_levelTimer.RemainingTime);
    }

    protected void OnDestroy()
    {
        if (_levelTimer)
            _levelTimer.OnTimeIsUp -= TimeIsUp;
    }

    private void TimeIsUp()
    {
        SetTimeText(0f);
    }

    private void SetTimeText(float time)
    {
        RemainingTimeText.text = time.ToString(TimeValueFormat, _formatInfo);
    }
}