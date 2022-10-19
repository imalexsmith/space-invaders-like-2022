using System;
using System.Collections;
using UnityEngine;
using UltEvents;
using NightFramework;


public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public struct DelayedSpawn
    {
        public Enemy EnemyPrefab;
        public float Delay;
        public bool InvertMovement;
    }


    // ====================================================
    public UltEvent<int> OnAllEnemiesKilled = new();

    [Space]
    public DelayedSpawn[] Enemies;

    [Space]
    public PoolableVFX SpawnVFXPrefab;
    
    public int RealEnemiesCount { get; private set; }

    private int _currentIndex;
    private int _killedCount;


    // ====================================================
    public void StartSpawn()
    {
        _currentIndex = 0;
        _killedCount = 0;

        if (Enemies.Length > 0)
            StartCoroutine(Spawning());
    }

    protected void Awake()
    {
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i].EnemyPrefab != null)
                RealEnemiesCount++;
        }
    }

    private IEnumerator Spawning()
    {
        while (_currentIndex <= Enemies.Length - 1)
        {
            yield return new WaitForSeconds(Enemies[_currentIndex].Delay);

            if (Enemies[_currentIndex].EnemyPrefab != null)
            {
                var enemy = MassSpawner2.Instance.Spawn(Enemies[_currentIndex].EnemyPrefab);
                enemy.transform.position = transform.position;
                if (Enemies[_currentIndex].InvertMovement)
                    enemy.InvertMovement();

                if (SpawnVFXPrefab != null)
                {
                    var vfx = MassSpawner2.Instance.Spawn(SpawnVFXPrefab);
                    vfx.transform.position = transform.position;
                }

                enemy.OnKilled += EnemyKilled;
            }

            _currentIndex++;
        }
    }

    private void EnemyKilled(Enemy enemy)
    {
        _killedCount++;
        enemy.OnKilled -= EnemyKilled;

        if (_killedCount == RealEnemiesCount)
            OnAllEnemiesKilled.Invoke(_killedCount);
    }
}