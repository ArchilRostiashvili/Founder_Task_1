using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.CrashAnalyticsSystem
{
    public abstract class CrashLoggerBase : MonoBehaviour, ICrashLogger
    {
        public abstract void Log(CrashMessage crashMessage);
        public abstract void LogException(CrashException crashException);
        public abstract void RecordEvent(ICrashEvent crashEvent);
        public abstract void SetCustomKey(CrashCustomKey crashCustomKey);
        public abstract void SetUserId(CrashUserId crashUserId);
    }
}
