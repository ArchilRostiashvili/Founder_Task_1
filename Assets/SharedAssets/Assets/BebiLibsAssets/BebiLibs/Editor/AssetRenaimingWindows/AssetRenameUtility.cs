
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BebiLibs.EditorExtensions.AssetRenaming
{
    public static class AssetRenameUtility
    {
        public static void ModifyAssetName(List<Object> objectList, AssetRenamingConfig renamingConfig)
        {
            foreach (var item in objectList)
            {
                string path = AssetDatabase.GetAssetPath(item);
                string fileName = Path.GetFileNameWithoutExtension(path);
                string newFileName = ApplyFilter(fileName, renamingConfig);
                item.name = newFileName;
                AssetDatabase.RenameAsset(path, newFileName);
            }
            AssetDatabase.SaveAssets();
        }

        public static string ApplyFilter(string value, AssetRenamingConfig renamingConfig)
        {
            string result = value;
            foreach (var item in renamingConfig.ReplaceList)
            {
                if (item.UseSmartReplace)
                {
                    result = result.ReplaceMultipleWithOne(item.Replace, item.ReplaceWith);
                }
                else
                {
                    result = result.Replace(item.Replace, item.ReplaceWith);
                }
            }

            if (renamingConfig.SelectedPrefix != AssetRenamingConfig.EMPTY_ELEMENT_KEY)
            {
                string prefixMatch = string.Empty;
                foreach (var item in renamingConfig.PrefixesList)
                {
                    if (result.StartsWith(item) && item.Length > prefixMatch.Length)
                    {
                        prefixMatch = item;
                    }
                }

                if (!string.IsNullOrEmpty(prefixMatch))
                {
                    result = result.Substring(prefixMatch.Length);
                }
            }

            if (renamingConfig.SelectedSuffix != AssetRenamingConfig.EMPTY_ELEMENT_KEY)
            {
                string suffixMatch = string.Empty;
                foreach (var item in renamingConfig.SuffixList)
                {
                    if (result.EndsWith(item) && item.Length > suffixMatch.Length)
                    {
                        suffixMatch = item;
                    }
                }

                if (!string.IsNullOrEmpty(suffixMatch))
                {
                    result = result.Substring(0, result.Length - suffixMatch.Length);
                }
            }


            if (!string.IsNullOrEmpty(renamingConfig.SelectedPrefix) && renamingConfig.SelectedPrefix != AssetRenamingConfig.EMPTY_ELEMENT_KEY)
            {
                result = renamingConfig.SelectedPrefix + result;
            }

            if (!string.IsNullOrEmpty(renamingConfig.SelectedSuffix) && renamingConfig.SelectedSuffix != AssetRenamingConfig.EMPTY_ELEMENT_KEY)
            {
                result += renamingConfig.SelectedSuffix;
            }


            if (renamingConfig.LowerCase)
            {
                result = result.ToLower();
            }

            return result;
        }

    }
}
