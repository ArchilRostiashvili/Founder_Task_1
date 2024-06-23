using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;

#if UNITY_EDITOR
using UnityEditor;

namespace CustomEditorUtilities
{
    public class UnityFileUtils : MonoBehaviour
    {
        public static bool IsTestableFile(string filePath)
        {
            return filePath.EndsWith(".meta") || filePath.EndsWith(".asset") || filePath.EndsWith(".unity") || filePath.EndsWith(".prefab") || filePath.EndsWith(".mat") || filePath.EndsWith(".cs") || filePath.EndsWith(".spriteatlas");
        }

        public static bool IsDirectory(string path)
        {
            try
            {
                return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public static string Colorize(string text, string color, bool bold = false)
        {
            return
            "<color=" + color + ">" +
            (bold ? "<b>" : "") +
            text +
            (bold ? "</b>" : "") +
            "</color>";
        }

        public static string GetMainFilePath(string metaFilePath)
        {
            string filenameWithMeta = Path.GetFileName(metaFilePath);
            return Path.Combine(Path.GetDirectoryName(metaFilePath), filenameWithMeta[..^5]);
        }

        public static string GetGuid(string filepath)
        {
            string guid = "";
            using (StreamReader reader = new StreamReader(filepath))
            {
                reader.ReadLine();
                guid = reader.ReadLine();
            }
            return guid;
        }

        public static bool FilesAreEqual(string firstPath, string secondPath)
        {
            byte[] firstHash, secondHash;

            using (FileStream first = new FileStream(firstPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                firstHash = MD5.Create().ComputeHash(first);
            }

            using (FileStream second = new FileStream(secondPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                secondHash = MD5.Create().ComputeHash(second);
            }

            for (int i = 0; i < firstHash.Length; i++)
            {
                if (firstHash[i] != secondHash[i])
                    return false;
            }
            return true;
        }

        public static string DuplicatePathFixer(string newFilePath)
        {
            if (File.Exists(newFilePath))
            {
                string fileName = Path.GetFileNameWithoutExtension(newFilePath);
                string fileExtension = Path.GetExtension(newFilePath);
                string filePath = Path.GetDirectoryName(newFilePath);
                string newFileName = fileName + "_1";
                string[] splitFileName = fileName.Split('_');
                if (splitFileName.Length > 1)
                {
                    string posableNumber = splitFileName[^1];
                    if (int.TryParse(posableNumber, out int result))
                    {
                        newFileName = fileName.Substring(0, fileName.Length - posableNumber.Length - 1) + "_" + (result + 1);
                    }
                }

                string finalPath = Path.Combine(filePath, newFileName + fileExtension);
                return DuplicatePathFixer(finalPath);
            }
            else
            {
                return newFilePath;
            }
        }

        public static List<T> FindScriptableOBjectByType<T>() where T : ScriptableObject
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }


        private static void DeleteFromDirectory(string startDirectory)
        {
            foreach (var directory in Directory.GetDirectories(startDirectory))
            {
                DeleteFromDirectory(directory);
                DeleteDirectory(directory);
            }
        }

        private static void DeleteDirectory(string directory)
        {
            int directoryFiles = Directory.GetFiles(directory).Length;
            int subDirectories = Directory.GetDirectories(directory).Length;

            if (directoryFiles == 0 && subDirectories == 0)
            {
                Directory.Delete(directory, true);
                string metaPath = directory + ".meta";
                if (File.Exists(metaPath))
                {
                    File.Delete(metaPath);
                }
            }
        }

        public static void RemoveDirectory(UnityFileData unityFileData)
        {
            if (!unityFileData.IsDirectory) return;

            try
            {
                if (Directory.Exists(unityFileData.FilePath))
                {
                    DeleteFromDirectory(unityFileData.FilePath);
                    DeleteDirectory(unityFileData.FilePath);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void RemoveFile(UnityFileData unityFile)
        {
            if (unityFile.IsDirectory) return;
            try
            {
                if (File.Exists(unityFile.MetaFilePath))
                {
                    File.Delete(unityFile.MetaFilePath);
                }

                if (File.Exists(unityFile.FilePath))
                {
                    File.Delete(unityFile.FilePath);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static bool TryConvertToProjectRelativePath(string folderPath, out string projectRelativePath)
        {
            if (IsProjectRelativePath(folderPath))
            {
                folderPath = Path.GetFullPath(folderPath);
                projectRelativePath = "Assets" + folderPath.Substring(Application.dataPath.Length);
                return true;
            }
            else
            {
                projectRelativePath = Path.GetFullPath(folderPath);
                return false;
            }
        }

        public static bool IsProjectRelativePath(string folderPath)
        {
            return Path.GetFullPath(folderPath).StartsWith(Path.GetFullPath(Application.dataPath));
        }
    }

}
#endif