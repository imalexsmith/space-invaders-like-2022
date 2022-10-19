using System;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;
using NightFramework;


public class LevelManager : Singleton<LevelManager>
{
    [Serializable]
    public struct SpawnWave
    {
        public EnemySpawner[] Spawners;
    }


    // ====================================================
    public UltEvent OnVictory = new();
    public UltEvent OnDefeat = new();
    public UltEvent<int> OnWaveStarted = new();

    [Space]
    public SpawnWave[] Waves;

    public int CurrentWave { get; private set; }
    
    private int _totalEnemiesCount;
    private int _killedEnemiesCount;
    private bool _levelFinished;

    private List<EnemySpawner> _activeSpawners = new();


    // ====================================================
    public void FinishLevel(bool result)
    {
        if (_levelFinished)
            return;

        _levelFinished = true;

        if (result)
            OnVictory.Invoke();
        else
            OnDefeat.Invoke();
    }

    protected void Start()
    {
        // nested loops are faster than using LINQ, but use more lines of code
        //_totalEnemiesCount = Waves.SelectMany(x => x.Spawners).SelectMany(x => x.Enemies).Where(x => x.EnemyPrefab != null).Count();

        foreach (var wave in Waves)
        {
            foreach (var spawner in wave.Spawners)
                _totalEnemiesCount += spawner.RealEnemiesCount;
        }

        SpawnCurrentWave();
    }

    private void SpawnCurrentWave()
    {
        foreach (var spawner in Waves[CurrentWave].Spawners)
        {
            _activeSpawners.Add(spawner);
            
            spawner.OnAllEnemiesKilled += CheckWave;
            spawner.StartSpawn();

            void CheckWave(int killed)
            {
                spawner.OnAllEnemiesKilled -= CheckWave;

                _killedEnemiesCount += killed;
                _activeSpawners.Remove(spawner);

                if (_killedEnemiesCount == _totalEnemiesCount)
                {
                    FinishLevel(true);
                    return;
                }

                if (_activeSpawners.Count == 0 && !_levelFinished)
                {
                    CurrentWave++;
                    SpawnCurrentWave();
                }
            }
        }

        OnWaveStarted.Invoke(CurrentWave);
    }
}