using UnityEngine;
using System.Collections.Generic;
using BebiLibs.Localization;
using BebiLibs.Localization.Core;

namespace BebiLibs.GameApplicationConfig
{
    [CreateAssetMenu(fileName = "ShareLinksConfig", menuName = "BebiLibs/ApplicationConfigs/ShareLinksConfig", order = 0)]
    public class ShareLinksConfig : ScriptableObject
    {
        [System.Serializable]
        public class DataShareLink
        {
            public LanguageIdentifier localLanguageType;
            public string shareSimple;
        }

        public List<DataShareLink> arrayDataShareLinks = new List<DataShareLink>();

        public string GetLocalizedShareLink()
        {
            LanguageIdentifier selectedLanguage = LocalizationManager.ActiveLanguage;
            return this.arrayDataShareLinks.Find(x => x.localLanguageType == selectedLanguage).shareSimple;
        }
    }
}
