using UnityEngine;
using UnityEngine.Pool;
using UltEvents;

// ========================
// Revision 2022.02.21
// ========================

namespace NightFramework
{
    public class PrimitivePoolable : MonoBehaviour, IPoolableObject
    {
        // ========================================================================================
        public UltEvent OnWakedUp = new();
        public UltEvent OnSlept = new();
        public UltEvent OnDispose = new();

        [Space]
        public bool IsManageGameObjectLifeCycle = true;

        public bool IsAwakened { get; protected set; }
        
        private bool _isDestroyed;
        public bool IsDestroyed => _isDestroyed;

        private IObjectPool<IPoolableObject> _pool;
        public IObjectPool<IPoolableObject> Pool 
        {
            get => _pool;
            set
            {
                if (_pool != null)
                    throw new UnityException();

                _pool = value;
            }
        }


        // ========================================================================================
        public virtual void ReturnToPool()
        {
            if (_isDestroyed || !IsAwakened)
                return;

            Pool?.Release(this);
        }

        public virtual void WakeUpPoolable(bool riseEvents = true)
        {
            if (_isDestroyed || IsAwakened)
                return;

            if (IsManageGameObjectLifeCycle)
                gameObject.SetActive(true);
            
            IsAwakened = true;
            if (riseEvents)
                OnWakedUp.InvokeSafe();
        }

        public virtual void ReleasePoolable(bool riseEvents = true)
        {
            if (_isDestroyed || !IsAwakened)
                return;

            if (riseEvents)
                OnSlept.InvokeSafe();
            IsAwakened = false;
            
            if (IsManageGameObjectLifeCycle)
                gameObject.SetActive(false);
        }

        public virtual void DisposePoolable(bool riseEvents = true)
        {
            if (_isDestroyed)
                return;

            if (riseEvents)
                OnDispose.InvokeSafe();

            if (IsManageGameObjectLifeCycle)
                Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            IsAwakened = false;
            _isDestroyed = true;
            _pool = null;
        }
    }
}