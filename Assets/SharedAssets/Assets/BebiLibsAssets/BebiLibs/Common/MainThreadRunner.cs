using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BebiLibs
{
    [ExecuteInEditMode]
    public class MainThreadRunner : MonoBehaviour
    {
        private static MainThreadRunner _instace;
        private static readonly Queue<ActionAsyncData> _actionsToExecute = new Queue<ActionAsyncData>();
        private static bool _initialized = false;

        public class ActionAsyncData
        {
            public System.Action action;
            public bool isExecuted;

            public ActionAsyncData(Action action)
            {
                this.action = action;
                this.isExecuted = false;
            }
        }

        private void Awake()
        {
            if (_instace == null)
            {
                _instace = this;
                _initialized = true;
            }
            else if (_instace != this)
            {
                GameObject.Destroy(this);
            }
        }

        public static async void RunInMainTread(System.Action action)
        {
            await ExecuteOnMainThread(action);
        }

        public static async Task ExecuteOnMainThread(System.Action action)
        {
            if (!_initialized)
            {
                Debug.LogError("Instance Of MainThreadRunner is null, Make Sure to Instantiate it In The Game Scene");
                return;
            }

            ActionAsyncData actionAsyncData = new ActionAsyncData(action);
            _actionsToExecute.Enqueue(actionAsyncData);

            while (actionAsyncData.isExecuted == false)
            {
                await Task.Yield();
            }
        }

        private void Update()
        {
            if (_actionsToExecute.Count > 0)
            {
                ActionAsyncData asyncAction = _actionsToExecute.Dequeue();
                if (asyncAction != null)
                {
                    asyncAction.isExecuted = true;
                    asyncAction.action?.Invoke();
                }
            }
        }

    }
}

