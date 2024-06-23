using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BebiLibs.Localization.Core;

namespace BebiLibs.Localization
{
    [ExecuteInEditMode]
    public class OnLocalizationAction : MonoBehaviour
    {
        public List<LanguageIdentifier> arrayLanguagesToMatch = new List<LanguageIdentifier>();
        public UnityEvent onLanguageMatch;
        public UnityEvent onLanguageNotMatch;

        public void UpdateState(LanguageIdentifier languageIdentifier)
        {
            if (arrayLanguagesToMatch.Contains(languageIdentifier))
            {
                onLanguageMatch?.Invoke();
            }
            else
            {
                onLanguageNotMatch?.Invoke();
            }
        }

        private void OnEnable()
        {
            LocalizationManager.OnLanguageChangeEvent -= UpdateState;
            LocalizationManager.OnLanguageChangeEvent += UpdateState;
            UpdateState(LocalizationManager.ActiveLanguage);
        }

        private void OnDisable()
        {
            LocalizationManager.OnLanguageChangeEvent -= UpdateState;
        }
    }
}
