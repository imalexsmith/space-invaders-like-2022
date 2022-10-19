using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

// ========================
// Revision 2022.02.21
// ========================

namespace NightFramework
{
    public class MassSpawner2 : Singleton<MassSpawner2>
    {
        // ========================================================================================
        public const int DEFAULT_CAPACITY = 10;


        // ========================================================================================
        public List<GameObject> Preload = new();

        private readonly Dictionary<GameObject, ObjectPool<IPoolableObject>> _pools = new();


        // ========================================================================================
        public void ClearAll()
        {
            foreach (var pool in _pools)
                pool.Value.Clear();

            _pools.Clear();
        }

        public void Clear<T>(T prefab) where T : MonoBehaviour, IPoolableObject
        {
            if (_pools.TryGetValue(prefab.gameObject, out var pool))
                pool.Clear();
        }

        public T Spawn<T>(T prefab) where T : MonoBehaviour, IPoolableObject
        {
            if (!_pools.TryGetValue(prefab.gameObject, out var pool))
                pool = InitializePool(prefab.gameObject, DEFAULT_CAPACITY);

            return pool.Get() as T;
        }

        protected void Reset()
        {
            ForgetInstanceOnDisable = false;
        }

        protected override void Awake()
        {
            base.Awake();

            if (IsInitialized)
            {
                foreach (var prefab in Preload)
                {
                    if (prefab != null)
                        InitializePool(prefab, DEFAULT_CAPACITY);
                }

                if (IsPermanentObject)
                    SceneManager.sceneUnloaded += OnSceneUnloaded;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearAll();
        }

        private void OnSceneUnloaded(Scene scene)
        {
            ClearAll();
        }

        private ObjectPool<IPoolableObject> InitializePool(GameObject prefab, int defaultCount)
        {
            var pool = new ObjectPool<IPoolableObject>(create, ActionOnGet, ActionOnRelease, ActionOnDestroy, defaultCapacity: defaultCount);
            _pools.Add(prefab, pool);

            IPoolableObject create()
            {
                var instance = Instantiate(prefab);
                if (!instance.TryGetComponent<IPoolableObject>(out var poolable))
                    poolable = instance.AddComponent<PrimitivePoolable>();
                poolable.Pool = _pools[prefab];
                return poolable;
            };

            pool.Release(pool.Get());

            return pool;
        }

        private void ActionOnGet(IPoolableObject target)
        {
            if (!target.IsDestroyed)
                target.WakeUpPoolable();
        }

        private void ActionOnRelease(IPoolableObject target)
        {
            if (!target.IsDestroyed)
                target.ReleasePoolable();
        }

        private void ActionOnDestroy(IPoolableObject target)
        {
            if (!target.IsDestroyed)
                target.DisposePoolable(!IsDestroyed);
        }
    }
}