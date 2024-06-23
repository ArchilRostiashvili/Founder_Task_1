using System.Collections.Generic;
using BebiLibs.Localization.Core;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.Localization.Data
{
    using System.Linq;
    using UnityEngine;
    [CreateAssetMenu(fileName = "ManagerLocalizationConfig", menuName = "BebiLibs/Localization/ManagerLocalizationConfig", order = 0)]
    public class LocalizationManagerConfig : ScriptableConfig<LocalizationManagerConfig>
    {
        [SerializeField] private bool _forceCurrent = false;
        [SerializeField] private LanguageIdentifier _defaultLanguageID;
        [SerializeField] private LanguageIdentifier _english;

        [Header("Language Data List")]
        [SerializeField] private List<LanguageIdentifier> _languageIdentifierList = new List<LanguageIdentifier>();

        [Header("Selected Language ID")]
        [SerializeField] private LanguageIdentifier _selectedLanguageID;

        //Will get deprecated from version 9.07.83 in favor of _persistentLanguageValue"
        private readonly PersistentInteger _languageTypeStore = new PersistentInteger("LanguageID", 0);
        private readonly PersistentString _persistentLanguageValue = new PersistentString("SelectedLanguageID", "");

        public bool ForceCurrent => _forceCurrent;

        public LanguageIdentifier DefaultLanguageIdentifier => _defaultLanguageID;
        public LanguageIdentifier ActiveLanguage => _selectedLanguageID;
        public List<LanguageIdentifier> LanguageIdentifierList => _languageIdentifierList;

        public LanguageIdentifier English => _english;


        public override void Initialize()
        {
            InitializeActiveLanguage();
        }

        public void InitializeActiveLanguage()
        {
            if (_forceCurrent)
            {
                _selectedLanguageID = _defaultLanguageID;
                SetSelectedLanguage(_defaultLanguageID);
            }

            if (!_languageTypeStore.isInitialized)
            {
                SetSelectedLanguage(GetApplicationLanguage());
            }
            else
            {
                int index = _languageTypeStore.GetValue();
                LanguageIdentifier languageIdentifier = index != -1 ? _languageIdentifierList[index] : English;
                SetSelectedLanguage(languageIdentifier);
            }
        }

        public void SetSelectedLanguage(string universalLanguageID)
        {
            LanguageIdentifier languageIdentifier = _languageIdentifierList.Find(x => x.UniversalLanguageID == universalLanguageID);
            SetSelectedLanguage(languageIdentifier ?? English);
        }

        public void SetSelectedLanguage(LanguageIdentifier languageIdentifier)
        {
            _selectedLanguageID = languageIdentifier;
            _persistentLanguageValue.SetValue(_selectedLanguageID.UniversalLanguageID);

            int index = _languageIdentifierList.FindIndex(x => x == languageIdentifier);
            _languageTypeStore.SetValue(index);
        }

        public LanguageIdentifier GetLanguageFromUniversalID(string universalLanguageID)
        {
            LanguageIdentifier dataLanguageInfo = _languageIdentifierList.Find(x => x.UniversalLanguageID == universalLanguageID);
            return dataLanguageInfo ?? English;
        }

        public LanguageIdentifier GetLanguageFromCode(string languageName)
        {
            LanguageIdentifier dataLanguageInfo = _languageIdentifierList.Find(x => x.LanguageName == languageName);
            return dataLanguageInfo ?? English;
        }

        private LanguageIdentifier GetApplicationLanguage()
        {
            LanguageIdentifier dataLanguageInfo = _languageIdentifierList.Find(x => x.SystemLanguage == Application.systemLanguage);
            return dataLanguageInfo ?? English;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(LocalizationManagerConfig))]
    public class LocalizationManagerConfigEditor : Editor
    {
        private LocalizationManagerConfig _localizationManagerConfig;
        private string[] _optionsArray = new string[0];
        private int _selectedIndex = 0;
        private int _lastSelectedIndex = 0;

        private void OnEnable()
        {
            _localizationManagerConfig = (LocalizationManagerConfig)target;
            _optionsArray = _localizationManagerConfig.LanguageIdentifierList.Select(x => x.LanguageName).ToArray();
            _selectedIndex = _localizationManagerConfig.LanguageIdentifierList.IndexOf(_localizationManagerConfig.ActiveLanguage);
            _lastSelectedIndex = _selectedIndex;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _selectedIndex = EditorGUILayout.Popup("Set Active Language: ", _selectedIndex, _optionsArray);
            if (_lastSelectedIndex != _selectedIndex)
            {
                int count = _localizationManagerConfig.LanguageIdentifierList.Count;
                if (_lastSelectedIndex >= 0 && _lastSelectedIndex < count)
                {
                    LanguageIdentifier identifier = _localizationManagerConfig.LanguageIdentifierList[_selectedIndex];
                    LocalizationManager.SetActiveLanguage(identifier);
                    _lastSelectedIndex = _selectedIndex;
                }
            }
        }
    }
#endif
}
