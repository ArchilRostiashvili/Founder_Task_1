using BebiLibs.RemoteConfigSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BebiLibs.RemoteListener
{
    public class RemoteListener : MonoBehaviour
    {
        public string remoteID;
        public bool targetValue;

        public UnityEvent OnValueEquals;
        public UnityEvent OnValueNotEquals;

        private void OnEnable()
        {
            RemoteConfigManager.OnLoadFinishedEvent += OnRemoteConfigUpdate;
            OnRemoteConfigUpdate();
        }

        private void OnDisable()
        {
            RemoteConfigManager.OnLoadFinishedEvent -= OnRemoteConfigUpdate;
        }

        public void OnRemoteConfigUpdate()
        {
            if (RemoteConfigManager.IsLoadFinished)
            {
                if (RemoteConfigManager.TryGetBool(remoteID, out bool value))
                {
                    if (value == targetValue)
                    {
                        OnValueEquals?.Invoke();
                    }
                    else
                    {
                        OnValueNotEquals?.Invoke();
                    }
                }
            }
        }
    }
}
