using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Collections;
using System.Linq;

namespace BebiLibs.CrashAnalyticsSystem
{
    public class CrashSimpleLogger : CrashLoggerBase
    {

        [SerializeField] private bool _enableLogging = false;
        private Queue<ICrashEvent> _gameEvents = new Queue<ICrashEvent>();
        private StringBuilder _parameterBuilder = new StringBuilder();

        private void OnEnable()
        {
            if (_enableLogging && Application.isPlaying)
            {
                InvokeRepeating("InvokeLog", 0.0f, 0.05f);
            }
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                CancelInvoke("InvokeLog");
            }
        }

        private void InvokeLog()
        {
            if (_gameEvents.Count > 0)
            {
                ICrashEvent gameEvent = _gameEvents.Dequeue();
                gameEvent.InvokeLog(this);
            }
        }

        public override void RecordEvent(ICrashEvent gameEvent)
        {
            if (_enableLogging)
            {
                _gameEvents.Enqueue(gameEvent);
            }
        }

        public override void Log(CrashMessage crashMessage)
        {
            Debug.LogError($"Crash Message : {crashMessage.Message}");
        }

        public override void LogException(CrashException crashException)
        {
            Debug.LogError($"Crash Exception : {crashException.Exception}");
        }

        public override void SetCustomKey(CrashCustomKey crashCustomKey)
        {
            Debug.LogError($"Crash Set Custom Key /n Key : {crashCustomKey.Key},/n Value: {crashCustomKey.Value}");
        }

        public override void SetUserId(CrashUserId crashUserId)
        {
            Debug.LogError($"Crash Set User Id : {crashUserId.UserId}");
        }
    }
}