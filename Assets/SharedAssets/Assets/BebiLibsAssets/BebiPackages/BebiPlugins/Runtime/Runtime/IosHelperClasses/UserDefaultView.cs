using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace BebiLibs
{
    public static class UserDefaultView
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        extern static void initSharingSystem(string groupID);
        [DllImport("__Internal")]
        extern static void writeSharedString(string key, string value);
        [DllImport("__Internal")]
        extern static string readSharedString(string key, string defaultValue);
        [DllImport("__Internal")]
        extern static string readSharedStringFromOtherApp(string bundleID, string key, string defaultValue);
#endif

        public static void InitSharingSystem(string groupID)
        {
#if UNITY_IOS
            initSharingSystem(groupID);
#endif
        }

        public static void WriteSharedString(string key, string value)
        {
#if UNITY_IOS
            //Debug.Log("Start Writing Shared String");
            writeSharedString(key, value);
#endif
        }

        public static string ReadSharedString(string key, string defaultValue)
        {
#if UNITY_IOS
            //Debug.Log("Start Reading Shared String");
            return readSharedString(key, defaultValue);
#else
            return string.Empty;
#endif
        }

        public static string ReadSharedStringFromOtherApp(string bundleID, string key, string defaultValue)
        {
#if UNITY_IOS
            //Debug.Log("Start Reading Shared String From Other App");
            return readSharedStringFromOtherApp(bundleID, key, defaultValue);
#else
            return string.Empty;
#endif
        }
    }
}
