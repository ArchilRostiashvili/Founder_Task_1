using System;
using UnityEditor;

namespace CustomEditorBuildWindow
{
    [System.Serializable]
    public class GameBuildVersionData
    {
        public string BuildVersionName;
        public BuildTarget AppBuildTarget;
        public string VersionString;
        public int BuildNumber;

        public string FullVersionString => $"{VersionString}.{BuildNumber}";

        //Verify if newVersion is valid 
        //Do not allow any non-numeric characters except for the first dot (.)
        //Do not allow more than 2 dots (.)
        //Do not allow more than 3 numbers before, middle and after the dots (.)
        //Return 0.0.0 if VersionString is null or empty
        public static string ValidateVersionString(string newVersion)
        {
            if (string.IsNullOrEmpty(newVersion))
            {
                return "0.0.0";
            }

            string[] split = newVersion.Split('.');
            if (split.Length > 3)
            {
                return newVersion[..^1];
            }

            if (newVersion.EndsWith("."))
            {
                return newVersion;
            }

            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].Length > 3 || !int.TryParse(split[i], out int result))
                {
                    return newVersion[..^1];
                }
            }
            return newVersion;
        }

        public static int ValidateBuildNumber(int newValue)
        {
            if (newValue < 0)
            {
                return 0;
            }
            return newValue;
        }
    }
}
