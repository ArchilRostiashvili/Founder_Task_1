#if UNITY_EDITOR
#endif

namespace BebiLibs.Localization.Core
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "LanguageIdentifier", menuName = "BebiLibs/LanguageIdentifier", order = 0)]
    public class LanguageIdentifier : ScriptableObject
    {
        [SerializeField] private SystemLanguage _systemLanguage;
        [SerializeField] private string _languageID;
        [SerializeField] private string _universalLanguageID;
        [SerializeField] private string _localizationAudioSuffix;
        [SerializeField] private string _languageName;
        [SerializeField] private Sprite _languageIconSprite;

        public SystemLanguage SystemLanguage => _systemLanguage;
        public string LanguageID => _languageID;
        public string UniversalLanguageID => _universalLanguageID;
        public string LocalizationAudioSuffix => _localizationAudioSuffix;
        public string LanguageName => _languageName;
        public Sprite LanguageIcon => _languageIconSprite;

    }


}
