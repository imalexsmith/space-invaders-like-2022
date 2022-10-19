using UnityEngine;
using TMPro;


public class LevelInfoView : MonoBehaviour
{
    // ====================================================
    public GameObject[] OnVictoryActivate;
    public GameObject[] OnDefeatActivate;
    public TMP_Text CurrentWaveText;
    public TMP_Text TotalWaveCountText;

    private LevelManager _levelManager;


    // ====================================================
    protected void Start()
    {
        _levelManager = LevelManager.Instance;

        TotalWaveCountText.text = _levelManager.Waves.Length.ToString();
        
        _levelManager.OnWaveStarted += WaveStarted;
        _levelManager.OnVictory += Victory;
        _levelManager.OnDefeat += Defeat;
    }

    protected void OnDestroy()
    {
        if (_levelManager)
        {
            _levelManager.OnWaveStarted -= WaveStarted;
            _levelManager.OnVictory -= Victory;
            _levelManager.OnDefeat -= Defeat;
        }
    }

    private void WaveStarted(int wave)
    {
        CurrentWaveText.text = (wave + 1).ToString();
    }

    private void Victory()
    {
        foreach (var go in OnVictoryActivate)
            go.SetActive(true);
    }

    private void Defeat()
    {
        foreach (var go in OnDefeatActivate)
            go.SetActive(true);
    }
}