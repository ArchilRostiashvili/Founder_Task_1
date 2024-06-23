using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BebiLibs
{
    public abstract class AbstractExitButton : MonoBehaviour
    {
        [SerializeField] protected UnityEvent _gameExitEvent;


        protected virtual void Initialize()
        {

        }

        public virtual void SetListener(UnityEvent unityEvent)
        {
            _gameExitEvent = unityEvent;
        }

        /// <summary>
        /// Add Listener To OnGameExit Event
        /// </summary>
        /// <param name="exitAction">Mathod name to execute that satisfies UnityAction delegate schema</param>
        public virtual void AddListener(UnityAction exitAction)
        {
            _gameExitEvent.AddListener(exitAction);
        }

        public void ForceExecute()
        {
            _gameExitEvent.Invoke();
        }

        /// <summary>
        /// Remove Listener From OnGameExit Event
        /// </summary>
        /// <param name="exitAction">Mathod name to remove from OnGameExit event that satisfies UnityAction delegate schema</param>
        public virtual void RemoveListener(UnityAction exitAction)
        {
            _gameExitEvent.RemoveListener(exitAction);
        }

        public virtual void RemoveAllListeners()
        {
            _gameExitEvent.RemoveAllListeners();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
