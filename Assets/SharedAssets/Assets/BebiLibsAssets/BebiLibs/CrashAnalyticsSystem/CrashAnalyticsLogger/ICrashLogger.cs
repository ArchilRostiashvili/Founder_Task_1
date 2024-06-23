using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.CrashAnalyticsSystem
{
    public interface ICrashLogger
    {
        public void RecordEvent(ICrashEvent crashEvent);
        public void SetCustomKey(CrashCustomKey crashCustomKey);
        public void Log(CrashMessage crashMessage);
        public void LogException(CrashException crashException);
        public void SetUserId(CrashUserId crashUserId);
    }
}
