using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.CrashAnalyticsSystem
{
    public interface ICrashEvent
    {
        void InvokeLog(ICrashLogger iCrashLogger);
    }
}
