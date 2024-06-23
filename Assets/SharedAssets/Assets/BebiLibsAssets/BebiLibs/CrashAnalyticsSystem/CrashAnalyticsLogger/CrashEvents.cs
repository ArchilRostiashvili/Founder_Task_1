using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.CrashAnalyticsSystem
{
    public class CrashCustomKey : ICrashEvent
    {
        public string Key;
        public string Value;

        public CrashCustomKey(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public void InvokeLog(ICrashLogger iCrashLogger)
        {
            iCrashLogger.SetCustomKey(this);
        }
    }


    public class CrashMessage : ICrashEvent
    {
        public string Message;

        public CrashMessage(string message)
        {
            Message = message;
        }

        public void InvokeLog(ICrashLogger iCrashLogger)
        {
            iCrashLogger.Log(this);
        }
    }

    public class CrashException : ICrashEvent
    {
        public System.Exception Exception;

        public CrashException(System.Exception exception)
        {
            Exception = exception;
        }

        public void InvokeLog(ICrashLogger iCrashLogger)
        {
            iCrashLogger.LogException(this);
        }
    }

    public class CrashUserId : ICrashEvent
    {
        public string UserId;

        public CrashUserId(string userId)
        {
            UserId = userId;
        }

        public void InvokeLog(ICrashLogger iCrashLogger)
        {
            iCrashLogger.SetUserId(this);
        }
    }
}
