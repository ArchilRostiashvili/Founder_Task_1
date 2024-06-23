using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace BebiLibs.CrashAnalyticsSystem
{
    public class CrashAnalyticsManager : GenericSingletonClass<CrashAnalyticsManager>
    {
        private static List<ICrashLogger> _EventLoggersList = new List<ICrashLogger>();

        [SerializeField] private List<CrashLoggerBase> _loggerBaseList = new List<CrashLoggerBase>();

        protected override void OnInstanceAwake()
        {
            for (int i = 0; i < _loggerBaseList.Count; i++)
            {
                RegisterEventListener(_loggerBaseList[i]);
            }
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void RegisterEventListener(ICrashLogger iEventLogger)
        {
            if (!_EventLoggersList.Contains(iEventLogger))
            {
                _EventLoggersList.Add(iEventLogger);
            }
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void UnregisterEventListener(ICrashLogger iEventLogger)
        {
            if (_EventLoggersList.Contains(iEventLogger))
            {
                _EventLoggersList.Remove(iEventLogger);
            }
        }

        private static void RecordEvent(ICrashEvent crashEvent)
        {
#if ACTIVATE_ANALYTICS
            foreach (ICrashLogger eventLogger in _EventLoggersList)
            {
                eventLogger.RecordEvent(crashEvent);
            }
#endif
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        public static void Log(string message)
        {
            CrashMessage crashMessage = new CrashMessage(message);
            RecordEvent(crashMessage);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void LogException(System.Exception exception)
        {
            CrashException crashException = new CrashException(exception);
            RecordEvent(crashException);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void SetCustomKey(string key, string value)
        {
            CrashCustomKey crashCustomKey = new CrashCustomKey(key, value);
            RecordEvent(crashCustomKey);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void SetUserId(string userId)
        {
            CrashUserId crashUserId = new CrashUserId(userId);
            RecordEvent(crashUserId);
        }
    }
}
