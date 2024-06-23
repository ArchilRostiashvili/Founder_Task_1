using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CustomEditorUtilities
{
    public class CommandRunner : MonoBehaviour
    {
        public static string RunCommand(string commandToRun, string workingDirectory = null)
        {
            if (string.IsNullOrEmpty(workingDirectory))
            {
                workingDirectory = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            }

            var processStartInfo = new System.Diagnostics.ProcessStartInfo()
            {
#if UNITY_EDITOR_WIN
                FileName = "cmd",
#elif UNITY_EDITOR_OSX
            FileName = "/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal",
#endif
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                //CreateNoWindow = true
            };

            var process = System.Diagnostics.Process.Start(processStartInfo);

            if (process == null)
            {
                throw new System.Exception("Process should not be null.");
            }

            process.StandardInput.WriteLine($"{commandToRun} & exit");
            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();
            //Debug.Log(output);
            return output;
        }
    }

}
