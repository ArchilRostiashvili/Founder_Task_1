using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace BebiLibs
{
    public static class CheckAppInstallation
    {
#if UNITY_IOS
    [DllImport("__Internal")]
    extern static bool isAppInstalled(string appSchema);
#endif

        public static bool IsAppInstalled(string appSchema)
        {
#if UNITY_IOS
        return isAppInstalled(appSchema);
#else
            return false;
#endif
        }

    }
}
