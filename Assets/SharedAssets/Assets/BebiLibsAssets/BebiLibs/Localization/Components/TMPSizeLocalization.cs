using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BebiLibs.Localization.Core;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.Localization
{
    public class TMPSizeLocalization : BaseLocalizationComponent
    {
        [System.Serializable]
        public class LanguageProperties : BaseLanguageProperties
        {
            [Header("Auto Parameters: ")]
            public bool AutoSize;
            public float MinSize;
            public float MaxSize;

            [Header("Font Size: ")]
            public float FontSize;
            public float CharSpace;
            public float LineSpace;

            public bool Bold;
        }

        [SerializeField] private LanguageProperties _baseSizeData;
        [SerializeField] private List<LanguageProperties> _languageData = new List<LanguageProperties>();

        public override BaseLanguageProperties this[int index] => _languageData[index];
        public override int dataCount => _languageData.Count;
        public override int activeIndex => _languageData.FindIndex(x => x.languageKey == LocalizationManager.ActiveLanguage.LanguageName);

        private TMP_Text _textComponent;
        private LanguageProperties empty = new LanguageProperties();

        public override void LoadDefaults()
        {
            if (!_textComponent) return;

            _baseSizeData.FontSize = _textComponent.fontSize;
            _baseSizeData.CharSpace = _textComponent.characterSpacing;
            _baseSizeData.LineSpace = _textComponent.lineSpacing;
            _baseSizeData.Bold = (int)(_textComponent.fontStyle & FontStyles.Bold) > 0;

            _baseSizeData.AutoSize = _textComponent.enableAutoSizing;
            _baseSizeData.MaxSize = _textComponent.fontSizeMax;
            _baseSizeData.MinSize = _textComponent.fontSizeMin;
        }

        public override void Init()
        {
            _textComponent = GetComponent<TMP_Text>();
        }

        public override void UpdateComponent(LanguageIdentifier languageIdentifier)
        {
            if (!_textComponent) return;

            LanguageProperties overrideLangrage = _languageData.Find(x => x.languageKey == I2.Loc.LocalizationManager.CurrentLanguage);

            if (overrideLangrage == null)
            {
                _textComponent.fontSize = _baseSizeData.FontSize;
                _textComponent.characterSpacing = _baseSizeData.CharSpace;
                _textComponent.lineSpacing = _baseSizeData.LineSpace;
                _textComponent.fontStyle = _baseSizeData.Bold ? _textComponent.fontStyle | FontStyles.Bold : _textComponent.fontStyle & ~FontStyles.Bold;

                _textComponent.enableAutoSizing = _baseSizeData.AutoSize;
                _textComponent.fontSizeMax = _baseSizeData.MaxSize;
                _textComponent.fontSizeMin = _baseSizeData.MinSize;
            }
            else
            {
                _textComponent.fontSize = _baseSizeData.FontSize + overrideLangrage.FontSize;
                _textComponent.characterSpacing = _baseSizeData.CharSpace + overrideLangrage.CharSpace;
                _textComponent.lineSpacing = _baseSizeData.LineSpace + overrideLangrage.LineSpace;
                _textComponent.fontStyle = overrideLangrage.Bold ? _textComponent.fontStyle | FontStyles.Bold : _textComponent.fontStyle & ~FontStyles.Bold;

                _textComponent.enableAutoSizing = overrideLangrage.AutoSize;
                _textComponent.fontSizeMax = _baseSizeData.MaxSize + overrideLangrage.MaxSize;
                _textComponent.fontSizeMin = _baseSizeData.MinSize = overrideLangrage.MinSize;
            }
        }

        public override bool IsChanged()
        {
            if (!_textComponent) return false;
            LanguageProperties property = _languageData.Find(x => x.languageKey == I2.Loc.LocalizationManager.CurrentLanguage);

            if (property == null) return false;

            bool font = _textComponent.fontSize != property.FontSize;
            bool space = _textComponent.characterSpacing != property.CharSpace;
            bool line = _textComponent.lineSpacing != property.LineSpace;

            TMPro.FontStyles style = property.Bold ? FontStyles.Bold : ~FontStyles.Bold;
            bool fStyle = _textComponent.fontStyle != style;
            return font || space || fStyle || line;
        }

        public override void LoadDefaultToActiveLanguage(string languageCode)
        {
            Debug.Log("Load Defaults");
        }

        public void AddLanguageOverride(string languageKey)
        {
            _languageData.Add(new LanguageProperties()
            {
                AutoSize = _baseSizeData.AutoSize,
                languageKey = languageKey
            });
        }

        public override void AddMissingLanguages()
        {

        }

        public bool HasOverrideLanguage(string language)
        {
            return _languageData.Find(x => x.languageKey == language) != null;
        }

        public override void RetrieveAllLanguages()
        {
            Init();
            LoadDefaults();
        }

        internal void RemoveLanguageOverride(string v)
        {
            int index = _languageData.FindIndex(x => x.languageKey == v);
            if (index != -1)
            {
                _languageData.RemoveAt(index);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TMPSizeLocalization))]
    public class TMPSizeLocalizationEditor : Editor
    {
        private TMPSizeLocalization _tmpSizeLocalization;
        private string[] _options;
        private int _selectedIndex;

        private void OnEnable()
        {
            _tmpSizeLocalization = (TMPSizeLocalization)target;

            List<string> localLanguageList = I2.Loc.LocalizationManager.GetAllLanguages();
            List<string> localLanguages = localLanguageList.Where(x => !_tmpSizeLocalization.HasOverrideLanguage(x)).ToList();

            _options = new string[localLanguages.Count];
            for (int i = 0; i < localLanguages.Count; i++)
            {
                _options[i] = localLanguages[i];
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            if (DrawDefaultInspector())
            {
                _tmpSizeLocalization.UpdateComponent(null);
            }

            _selectedIndex = EditorGUILayout.Popup("Overridable Language", _selectedIndex, _options);

            if (GUILayout.Button("Add Selected Language As Override"))
            {
                _tmpSizeLocalization.AddLanguageOverride(_options[_selectedIndex]);
            }

            if (GUILayout.Button("Update Default Data"))
            {
                _tmpSizeLocalization.LoadDefaults();
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
#endif
}
