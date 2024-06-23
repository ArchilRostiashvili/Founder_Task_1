#if ACTIVATE_REMOTE_CONFIG
using Firebase.RemoteConfig;
#endif

using UnityEngine;
namespace BebiLibs.RemoteConfigSystem.Variables
{
    public static class ConfigValuePrinter
    {
#if ACTIVATE_REMOTE_CONFIG
        public static bool TryGetRemoteVariableFromConfig(string key, ConfigValue configValue, out RemoteVariable remoteVariable)
        {
            remoteVariable = null;
            if (TryGetBooleanValue(configValue, out bool boolValue))
            {
                remoteVariable = RemoteBoolVariable.Create(key, boolValue);
                return true;
            }
            else if (TryGetStringValue(configValue, out string stringValue))
            {
                remoteVariable = RemoteStringVariable.Create(key, stringValue);
                return true;
            }
            else if (TryGetLongValue(configValue, out long longValue))
            {
                remoteVariable = RemoteLongVariable.Create(key, longValue);
                return true;
            }
            else if (TryGetDoubleValue(configValue, out double doubleValue))
            {
                remoteVariable = RemoteDoubleVariable.Create(key, doubleValue);
                return true;
            }
            return false;
        }


        public static bool TryGetBooleanValue(ConfigValue configValue, out bool value)
        {
            try
            {
                value = configValue.BooleanValue;
                return true;
            }
            catch
            {
                value = false;
                return false;
            }
        }

        public static bool TryGetLongValue(ConfigValue configValue, out long value)
        {
            try
            {
                value = configValue.LongValue;
                return true;
            }
            catch
            {
                value = 0;
                return false;
            }
        }

        public static bool TryGetDoubleValue(ConfigValue configValue, out double value)
        {
            try
            {
                value = configValue.DoubleValue;
                return true;
            }
            catch
            {
                value = 0;
                return false;
            }
        }

        public static bool TryGetStringValue(ConfigValue configValue, out string value)
        {
            try
            {
                value = configValue.StringValue;
                return true;
            }
            catch
            {
                value = string.Empty;
                return false;
            }
        }
#endif
    }
}

