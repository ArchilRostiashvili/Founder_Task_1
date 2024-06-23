using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DependencyResolverInitializer
{
    [CreateAssetMenu(fileName = "DependencyManagerInitializedConfig", menuName = "BebiLibs/Helpers/DependencyManagerInitializedConfig", order = 0)]
    public class DependencyManagerInitializedConfig : ScriptableObject
    {
        public Object DependencyManagerAsset;
        public bool IsInitialized = false;

        private void OnDestroy()
        {
            IsInitialized = false;
            Debug.Log("DependencyManagerInitializedConfig OnDestroy");
        }
    }
}