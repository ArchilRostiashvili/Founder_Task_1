using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public abstract class GenericSingletonClass<T> : SingletonClass where T : GenericSingletonClass<T>
    {
        private static T _Instance;
        public static T Instance => DefaultInstance();
        public static bool IsInstanceValid => _Instance != null;

        protected static T DefaultInstance()
        {
            if (_Instance == null && Application.isPlaying)
            {
                GameObject obj = new GameObject(typeof(T).Name + " " + " Auto Generated Singleton");
                _Instance = obj.AddComponent<T>();
            }
            return _Instance;
        }


        private void Awake()
        {
            if (_Instance == null)
            {
                _Instance = (T)this;
                _Instance.OnInstanceAwake();
                if (_dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_Instance != this)
            {
                Debug.LogWarning($"Removing Duplicate Instance Of {typeof(T).Name}");
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            OnInstanceDestroy();
        }
    }

    public abstract class SingletonClass : MonoBehaviour
    {
        protected bool _dontDestroyOnLoad = false;

        protected virtual void OnInstanceAwake()
        {

        }

        protected virtual void OnInstanceDestroy()
        {

        }
    }
}
