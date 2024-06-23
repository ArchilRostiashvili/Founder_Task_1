using BebiLibs.EditorExtensions.AssetRenaming;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BebiLibs.EditorExtensions
{
    public static class ContextMenuHelper
    {
        public delegate void CreateAsset<T>(string folderPath, List<T> unityAssets) where T : UnityEngine.Object;

        public static void CreateObjectFromAssets<T>(CreateAsset<T> createAsset, AssetRenamingConfig assetRenamingConfig = null) where T : UnityEngine.Object
        {
            string folderPath = "Assets";
            if (Selection.assetGUIDs.Length > 0)
            {
                folderPath = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]));
            }

            string folder = EditorUtility.SaveFolderPanel("Save Folder", folderPath, "");

            if (string.IsNullOrEmpty(folder))
            {
                Debug.LogError("No folder selected");
                return;
            }

            if (!folder.StartsWith(Application.dataPath))
            {
                Debug.LogError("Folder must be inside the Assets folder");
                return;
            }

            folder = folder.Replace(Application.dataPath, "Assets");

            List<T> unityAssetList = new List<T>();
            foreach (string assetGUIDS in Selection.assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDS);
                if (CheckIfAssetMatchesType<T>(assetPath, out T unityAsset))
                {
                    unityAssetList.Add(unityAsset);
                }
            }
            unityAssetList.Sort((x, y) => x.name.CompareTo(y.name));
            List<Object> selectedObjects = new List<UnityEngine.Object>(unityAssetList.ToArray());

            if (assetRenamingConfig == null)
            {
                createAsset(folder, unityAssetList);
                return;
            }

            AssetRenamingWindows.ShowWindow(selectedObjects, assetRenamingConfig, () =>
            {
                createAsset(folder, unityAssetList);
            }, () =>
            {
                createAsset(folder, unityAssetList);
            });
        }

        public static bool CheckIfAssetMatchesType<T>(string path, out T unityAsset) where T : UnityEngine.Object
        {
            unityAsset = AssetDatabase.LoadAssetAtPath<T>(path);
            return unityAsset != null;
        }

        public static bool IfSelectionIsValidType<T>() where T : UnityEngine.Object
        {
            bool canUse = true;
            foreach (string assetGUIDS in Selection.assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDS);
                if (!CheckIfAssetMatchesType<T>(assetPath, out _))
                {
                    canUse = false;
                    break;
                }
            }
            return canUse;
        }

        public static bool IsSelectionOnlyDirectories()
        {
            bool isOnlyDirectories = true;
            foreach (string assetGUIDS in Selection.assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDS);
                if (!AssetDatabase.IsValidFolder(assetPath))
                {
                    isOnlyDirectories = false;
                    break;
                }
            }
            return isOnlyDirectories;
        }

        public static List<string> GetAssetPathFromSelection()
        {
            string[] assetGuids = Selection.assetGUIDs;
            List<string> assetPaths = assetGuids.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList();
            return assetPaths;
        }
    }
}
