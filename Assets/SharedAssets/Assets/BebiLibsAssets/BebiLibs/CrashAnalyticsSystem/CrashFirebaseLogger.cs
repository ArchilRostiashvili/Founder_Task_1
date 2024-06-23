using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if ACTIVATE_ANALYTICS
using BebiLibs.Analytics;
using Firebase.Analytics;
using Firebase;
using Firebase.Crashlytics;
#endif

namespace BebiLibs.CrashAnalyticsSystem
{
    public class CrashFirebaseLogger : CrashLoggerBase
    {
        [Range(20, 100)]
        [SerializeField] private int _maxStoredEventCount = 100;

        private Queue<ICrashEvent> _gameEvents = new Queue<ICrashEvent>();

        private bool _isFirebaseAvailable = false;

#if ACTIVATE_ANALYTICS
        private void Awake()
        {
            FirebaseDependencyResolver.AddInitializationListener(StartLogging, true);
        }

        private void OnDestroy()
        {
            FirebaseDependencyResolver.RemoveInitializationListener(StartLogging);
        }

        private void StartLogging(bool isFirebaseAvailable)
        {
            _isFirebaseAvailable = isFirebaseAvailable;
            if (isFirebaseAvailable)
            {
                InvokeRepeating(nameof(InvokeLog), 0.0f, Time.deltaTime);
            }
        }

        private void InvokeLog()
        {
            if (_gameEvents.Count > 0)
            {
                ICrashEvent gameEvent = _gameEvents.Dequeue();
                gameEvent.InvokeLog(this);
                DisableInvokeLogMethod();
            }
            else
            {
                CancelInvoke(nameof(InvokeLog));
            }
        }

        private void DisableInvokeLogMethod()
        {
            if (_gameEvents.Count == 0)
            {
                CancelInvoke(nameof(InvokeLog));
            }
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                CancelInvoke(nameof(InvokeLog));
            }
        }
#endif

        public override void RecordEvent(ICrashEvent gameEvent)
        {
#if ACTIVATE_ANALYTICS
            if (_isFirebaseAvailable && _gameEvents.Count == 0)
            {
                gameEvent.InvokeLog(this);
            }
            else
            {
                _gameEvents.Enqueue(gameEvent);
                if (_gameEvents.Count > _maxStoredEventCount)
                {
                    _gameEvents.Dequeue();
                }
            }
#endif
        }

        public override void Log(CrashMessage crashMessage)
        {
#if ACTIVATE_ANALYTICS
            try
            {
                Crashlytics.Log(crashMessage.Message);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
#endif
        }

        public override void LogException(CrashException crashException)
        {
#if ACTIVATE_ANALYTICS
            try
            {
                Crashlytics.LogException(crashException.Exception);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
#endif
        }

        public override void SetCustomKey(CrashCustomKey crashCustomKey)
        {
#if ACTIVATE_ANALYTICS
            try
            {
                Crashlytics.SetCustomKey(crashCustomKey.Key, crashCustomKey.Value);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
#endif
        }

        public override void SetUserId(CrashUserId crashUserId)
        {
#if ACTIVATE_ANALYTICS
            try
            {
                Crashlytics.SetUserId(crashUserId.UserId);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
#endif
        }
    }
}
