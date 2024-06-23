using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.EditorExtensions.AssetRenaming
{
    [SerializeField]
    public class AssetRenamingConfig : ScriptableObject
    {
        public const string EMPTY_ELEMENT_KEY = "Nothing";

        public List<string> PrefixesList = new List<string>();
        public List<string> SuffixList = new List<string>();
        [HideInInspector] public string SelectedPrefix = EMPTY_ELEMENT_KEY;
        [HideInInspector] public string SelectedSuffix = EMPTY_ELEMENT_KEY;
        [HideInInspector] public bool LowerCase = false;
        public List<ReplaceStringData> ReplaceList = new List<ReplaceStringData>();

        public static AssetRenamingConfig Create()
        {
            AssetRenamingConfig assetRenamingConfig = ScriptableConfig.CreateInstance<AssetRenamingConfig>();
            return assetRenamingConfig;
        }

        public static AssetRenamingConfig Create(List<string> prefixesList, List<string> suffixList, List<ReplaceStringData> replaceList, bool useLowerCase = false)
        {
            AssetRenamingConfig assetRenamingConfig = ScriptableConfig.CreateInstance<AssetRenamingConfig>();
            assetRenamingConfig.PrefixesList = prefixesList;
            assetRenamingConfig.SuffixList = suffixList;
            assetRenamingConfig.ReplaceList = replaceList;
            assetRenamingConfig.LowerCase = useLowerCase;

            if (!suffixList.Contains(EMPTY_ELEMENT_KEY))
            {
                suffixList.Insert(0, EMPTY_ELEMENT_KEY);
            }

            if (!prefixesList.Contains(EMPTY_ELEMENT_KEY))
            {
                prefixesList.Insert(0, EMPTY_ELEMENT_KEY);
            }

            return assetRenamingConfig;
        }
    }
}
