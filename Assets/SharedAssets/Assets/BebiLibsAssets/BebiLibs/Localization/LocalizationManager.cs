using UnityEngine;
using BebiLibs.Localization.Data;
using BebiLibs.Localization.Core;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.Localization
{
    public class LocalizationManager
    {
        public static System.Action<LanguageIdentifier> OnLanguageChangeEvent;

        private static LocalizationManagerConfig _Config => LocalizationManagerConfig.DefaultInstance();

        public static LanguageIdentifier ActiveLanguage => _Config.ActiveLanguage;

        public static LanguageIdentifier English => _Config.English;
        public static LanguageIdentifier Georgian => _Config.GetLanguageFromUniversalID("ka");

        public static string ActiveLanguageUniversalID => ActiveLanguage.UniversalLanguageID;
        public static string ActiveLanguageName => ActiveLanguage.LanguageName;
        public static string ActiveLanguageID => ActiveLanguage.LanguageID;
        public static Sprite ActiveLanguageSprite => ActiveLanguage.LanguageIcon;

        public static bool IsEnglish() => ActiveLanguage == English;

        public static void SetActiveLanguage(LanguageIdentifier languageIdentifier)
        {
            _Config.SetSelectedLanguage(languageIdentifier);
            I2.Loc.LocalizationManager.CurrentLanguage = _Config.ActiveLanguage.LanguageName;
            OnLanguageChangeEvent?.Invoke(_Config.ActiveLanguage);
        }

        public static LanguageIdentifier GetLanguageFromUniversalID(string universalLanguageID)
        {
            return _Config.GetLanguageFromUniversalID(universalLanguageID);
        }

        public static LanguageIdentifier GetLanguageFromName(string languageName)
        {
            return _Config.GetLanguageFromCode(languageName);
        }

        public static string GetTranslation(string term, bool fixForRTL = true, int maxLineLengthForRTL = 0, bool ignoreRTLnumbers = true, bool applyParameters = false, GameObject localParametersRoot = null, string overrideLanguage = null, bool allowLocalizedParameters = true)
        {
            TryGetTranslation(term, out string Translation, fixForRTL, maxLineLengthForRTL, ignoreRTLnumbers, applyParameters, localParametersRoot, overrideLanguage, allowLocalizedParameters);
            return Translation;
        }

        public static string GetTermTranslation(string term, bool fixForRTL = true, int maxLineLengthForRTL = 0, bool ignoreRTLnumbers = true, bool applyParameters = false, GameObject localParametersRoot = null, string overrideLanguage = null, bool allowLocalizedParameters = true)
        {
            return GetTranslation(term, fixForRTL, maxLineLengthForRTL, ignoreRTLnumbers, applyParameters, localParametersRoot, overrideLanguage, allowLocalizedParameters);
        }

        public static bool TryGetTranslation(string term, out string translation, bool fixForRTL = true, int maxLineLengthForRTL = 0, bool ignoreRTLnumbers = true, bool applyParameters = false, GameObject localParametersRoot = null, string overrideLanguage = null, bool allowLocalizedParameters = true)
        {
            return I2.Loc.LocalizationManager.TryGetTranslation(term, out translation, fixForRTL, maxLineLengthForRTL, ignoreRTLnumbers, applyParameters, localParametersRoot, overrideLanguage, allowLocalizedParameters);
        }

#if UNITY_EDITOR
        public static void SetDirty()
        {
            EditorUtility.SetDirty(LocalizationManagerConfig.DefaultInstance());
        }
#endif

    }
}
