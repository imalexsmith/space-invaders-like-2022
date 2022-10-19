using UnityEngine;
using UnityEngine.Pool;

// ========================
// Revision 2022.02.21
// ========================

namespace NightFramework
{
    public interface IPoolableObject
    {
#pragma warning disable IDE1006 // Naming Styles
        public GameObject gameObject { get; }
#pragma warning restore IDE1006 // Naming Styles

        public bool IsDestroyed { get; }
        public IObjectPool<IPoolableObject> Pool { get; set; }
        public void ReturnToPool();
        public void WakeUpPoolable(bool riseEvents = true);
        public void ReleasePoolable(bool riseEvents = true);
        public void DisposePoolable(bool riseEvents = true);
    }
}