using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing.Security;
namespace BebiLibs.GameSecurity
{
    internal class PrefKeyObfuscator : MonoBehaviour
    {
        private static byte[] data = System.Convert.FromBase64String("vYCdm4GNrbqEs6CmzJuxr6HDo4KCvcfCwL6ypb2/h7y+rJGBzaaSptqNgJq+lrCPv4SQraK4gb+Wv8PCpN6Qlr3Axo+Np56ShrqBhpGFpreErIKPrZqHko+FxseAjYDHp5+tur+Qn6ash7+8s8CBnpKZuqKPkY29sLS0tLSxtKS0t7S0tLeSpLGdgrLHsafBgbyU3pLag4SRlsSHxqOzr5ebnJS6xKaCoLickbmyg5q0paOdm7O0xpKEug==");
        private static int[] order = new int[] { 3, 8, 5, 8, 4, 5, 8, 7, 8, 9 };
        private static int key = 245;

        public static readonly bool IsPopulated = true;

        public static byte[] Data()
        {
            return Obfuscator.DeObfuscate(data, order, key);
        }

        public static string DataString()
        {
#if UNITY_EDITOR
            return Convert.ToBase64String(data);
#else
    return Convert.ToBase64String(Obfuscator.DeObfuscate(data, order, key));
#endif
        }

    }
}
#endif