using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.AudioSystem
{
    public abstract class AutocreatableInstance<T> : AutocreatableInstanceBase where T : AutocreatableInstance<T>
    {
        protected static T _instance;

        protected static T GetInstance(string PREFAB_PATH)
        {
            if (_instance == null)
            {
                GameObject prefab = Resources.Load<GameObject>(PREFAB_PATH);

                _instance = GameObject.Instantiate(prefab).GetComponent<T>();
                if (_instance == null)
                {
                    Debug.LogError($"Couldn't instantiate prefab of type {typeof(T)}");
                }
                else
                {
                    _instance.InitContent();
                }
            }

            return _instance;
        }
    }

    public abstract class AutocreatableInstanceBase : MonoBehaviour
    {
        public abstract void InitContent();
    }
}