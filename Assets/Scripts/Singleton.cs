using UnityEngine;

// ========================
// Revision 2022.10.04
// ========================

namespace NightFramework
{
    public abstract class Singleton<T> : Singleton where T : Singleton<T>
    {
        // ========================================================================================
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var candidate = FindObjectOfType<T>(true);
                    if (candidate == null)
                    {
                        var go = new GameObject($"{typeof(T).Name} Singleton");
                        candidate = go.AddComponent<T>();
                    }

                    candidate.Initialize();
                }

                return _instance;
            }
        }

        public static bool IsReady => _instance != null && _instance.IsInitialized && !_instance.IsDestroyed;


        // ========================================================================================
        public new bool IsInitialized
        {
            get => base.IsInitialized;
            private set => base.IsInitialized = value;
        }

        public new bool IsDestroyed 
        {
            get => base.IsDestroyed; 
            private set => base.IsDestroyed = value; 
        }


        // ========================================================================================
        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void OnEnable()
        {
            if (ForgetInstanceOnDisable)
                Initialize();
        }

        protected virtual void OnDisable()
        {
            if (ForgetInstanceOnDisable)
                ForgetInstance();
        }

        protected virtual void OnDestroy()
        {
            ForgetInstance();
            IsDestroyed = true;
        }

        private void Initialize()
        {
            if (_instance == null)
            {
                _instance = (T)this;
                if (IsPermanentObject)
                    DontDestroyOnLoad(gameObject);
                IsInitialized = true;
            }
            else if (_instance != this)
            {
                throw new UnityException($"There cannot be more than one {nameof(T)} script in the scene. The instances {_instance.name} and {name} were found.");
            }
        }

        private void ForgetInstance()
        {
            if (_instance == this)
            {
                IsInitialized = false;
                _instance = null;
            }
        }
    }

    public abstract class Singleton : MonoBehaviour
    {
        [Tooltip("If turned on, marks this GameObject as DontDestroyOnLoad when singleton becomes initialized, so it will be transfered between scenes. Otherwise singleton only lives in the current scene.")]
        public bool IsPermanentObject;
        [Tooltip("If turned on, the Instance reference will be set to null when the object is disabled. Otherwise will keep the Instance reference even when the object is disabled.")]
        public bool ForgetInstanceOnDisable = true;

        public virtual bool IsInitialized { get; protected set; }
        public virtual bool IsDestroyed { get; protected set; }
    }
}