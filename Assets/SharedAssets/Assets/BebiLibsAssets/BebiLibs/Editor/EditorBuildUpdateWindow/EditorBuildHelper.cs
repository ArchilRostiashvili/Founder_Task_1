using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CustomEditorBuildWindow
{
    [InitializeOnLoad]
    public class EditorBuildHelper : MonoBehaviour
    {
        static EditorBuildHelper()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuildPlayerButtonClick);
        }

        internal static void OnBuildPlayerButtonClick(BuildPlayerOptions options)
        {
            BuildVersionUpdateWindow.ShowWindow(options);
        }

        internal static BuildPlayerOptions GetDefaultBuildPlayerOptions()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            return BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(buildPlayerOptions);
        }

        internal static void BuildPlayer(BuildPlayerOptions options)
        {
            //Debug.Log("BuildPlayer " + options.locationPathName);
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
        }

        internal static string RenameBuildName(string buildPath, string buildVersionString)
        {
            string localPathName = buildPath;
            string directory = Path.GetDirectoryName(localPathName);
            string fileName = Path.GetFileNameWithoutExtension(localPathName);
            string extension = Path.GetExtension(localPathName);

            if (fileName.Contains("_"))
            {
                string[] split = fileName.Split('_');
                if (split.Length > 1 && Version.TryParse(split[^1], out Version _))
                {
                    fileName = string.Join("_", split.Take(split.Length - 1));
                }
            }

            fileName = fileName + "_" + buildVersionString;
            localPathName = Path.Combine(directory, fileName + extension);
            return localPathName;
        }
    }

}
