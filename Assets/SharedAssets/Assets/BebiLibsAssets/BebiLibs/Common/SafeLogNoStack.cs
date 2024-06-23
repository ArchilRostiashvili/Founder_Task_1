using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public static class SafeLogNoStack
    {
        public static void Log(string logText, string errorInfo = "")
        {
            try
            {
                logText = logText.Replace("{", "{{").Replace("}", "}}");
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, logText);
            }
            catch (Exception e)
            {
                Debug.Log($"Unable To Log {errorInfo}, Error: " + e);
            }
        }
    }
}
