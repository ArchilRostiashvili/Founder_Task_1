using UnityEngine;
using System.Collections.Generic;
using BebiLibs;
using System;
using UnityEngine.UI;
using BebiLibs.Localization.Core;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.Localization
{
    [ExecuteInEditMode]
    public abstract class BaseLocalizationComponent : MonoBehaviour, ISerializationCallbackReceiver
    {

        public abstract BaseLanguageProperties this[int index] { get; }
        public abstract int dataCount { get; }
        public abstract int activeIndex { get; }

        protected bool _isControlledByLayout = false;

        private void Start()
        {
            Init();
            UpdateComponent(LocalizationManager.ActiveLanguage);
        }

        private void CheckUILayoutComponent()
        {
            LayoutGroup layoutGroup = GetComponentInParent<LayoutGroup>();
            if (layoutGroup != null)
            {
                _isControlledByLayout = true;
            }
        }

        private void Reset()
        {
            Init();
            LoadDefaults();
        }

        public void OnEnable()
        {
            LocalizationManager.OnLanguageChangeEvent -= UpdateComponent;
            LocalizationManager.OnLanguageChangeEvent += UpdateComponent;
            CheckUILayoutComponent();
            UpdateComponent(LocalizationManager.ActiveLanguage);
        }

        public void OnDisable()
        {
            LocalizationManager.OnLanguageChangeEvent -= UpdateComponent;
            CheckUILayoutComponent();
        }

        public void ThrowDisabledExertion()
        {
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy)
            {
                throw new System.Exception("Element That You Are Trying To Update Is Disabled, May Cause Unexpected Behaviour, Enable it before editing");
            }
        }


        public abstract void Init();
        public abstract void UpdateComponent(LanguageIdentifier languageIdentifier);
        public abstract void LoadDefaults();
        public abstract void LoadDefaultToActiveLanguage(string code);
        public abstract bool IsChanged();

        public void LoadLanguage(string code)
        {
            LanguageIdentifier languageIdentifier = LocalizationManager.GetLanguageFromName(code);
            LocalizationManager.SetActiveLanguage(languageIdentifier);
            // = LocalizationData.GetDefaultInstance().GetLanguageFromCode(code);
            //ManagerLocalization.SetActiveLanguage(localLanguageType);
        }

        public abstract void RetrieveAllLanguages();
        public abstract void AddMissingLanguages();


        public void OnBeforeSerialize()
        {
            // if (IsChanged())
            // {
            //     //Debug.Log("Is Changed");
            //     string languageKey = this[activeIndex].languageKey;
            //     if (!gameObject.activeSelf || !gameObject.activeInHierarchy) return;
            //     LoadDefaultToActiveLanguage(languageKey);
            // }
        }

        public void OnAfterDeserialize()
        {

        }
    }

    [System.Serializable]
    public class BaseLanguageProperties
    {
        [HideInInspector]
        public string languageKey;
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BaseLocalizationComponent), true)]
    public class BaseLocalizationComponentEditor : Editor
    {
        private bool _showLanguages;
        public BaseLocalizationComponent textMeshProLocalization;
        public SerializedProperty langueArrayProperty;
        public List<SerializedProperty> arrayProperty = new List<SerializedProperty>();

        private string[] _options;
        private int _selectedIndex;

        protected virtual void OnEnable()
        {
            try
            {
                textMeshProLocalization = (BaseLocalizationComponent)target;
            }
            catch
            {
                textMeshProLocalization = null;
            }
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (textMeshProLocalization == null) return;
            if (textMeshProLocalization.dataCount == 0)
            {
                _selectedIndex = -1;
                return;
            }

            arrayProperty.Clear();
            SerializedProperty languageArray = serializedObject.FindProperty("_languageData");
            for (int i = 0; i < languageArray.arraySize; i++)
            {
                SerializedProperty prop = languageArray.GetArrayElementAtIndex(i);
                arrayProperty.Add(prop);
            }


            _selectedIndex = textMeshProLocalization.activeIndex;

            if (arrayProperty.Count > 0 && (_selectedIndex < 0 || _selectedIndex > arrayProperty.Count))
            {
                _selectedIndex = 0;
            }

            _options = new string[textMeshProLocalization.dataCount];
            for (int i = 0; i < textMeshProLocalization.dataCount; i++)
            {
                _options[i] = textMeshProLocalization[i].languageKey;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            //DrawDefaultInspector();
            if (textMeshProLocalization == null) return;

            if (_selectedIndex >= 0)
            {
                EditorGUI.BeginChangeCheck();
                _selectedIndex = EditorGUILayout.Popup(_selectedIndex, _options);
                if (EditorGUI.EndChangeCheck())
                {
                    textMeshProLocalization.LoadLanguage(textMeshProLocalization[_selectedIndex].languageKey);
                    textMeshProLocalization.UpdateComponent(LocalizationManager.ActiveLanguage);
                }
            }

            if (arrayProperty.Count == 0 || _selectedIndex >= arrayProperty.Count)
            {
                UpdateUI();
            }

            GUILayout.Space(4);

            if (arrayProperty.Count == 0)
            {
                EditorGUILayout.HelpBox("There are no localization data available, Press \"Load Languages Defalt Data\" button to load languages with default parameters", MessageType.Warning);
            }

            if (_selectedIndex >= 0 && arrayProperty.Count > 0 && arrayProperty[_selectedIndex] != null)
            {
                if (_selectedIndex > arrayProperty.Count)
                {
                    _selectedIndex = arrayProperty.Count - 1;
                }

                EditorGUI.BeginChangeCheck();
                arrayProperty[_selectedIndex].isExpanded = true;
                EditorGUILayout.PropertyField(arrayProperty[_selectedIndex], true);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(textMeshProLocalization, "Array Changed");
                    EditorUtility.SetDirty(textMeshProLocalization);
                    textMeshProLocalization.UpdateComponent(LocalizationManager.ActiveLanguage);
                }

                if (GUILayout.Button("Load Default Data To Selected Localization"))
                {
                    textMeshProLocalization.LoadDefaultToActiveLanguage(textMeshProLocalization[_selectedIndex].languageKey);
                    EditorUtility.SetDirty(textMeshProLocalization);
                    UpdateUI();
                }
            }

            GUILayout.Space(4);

            if (GUILayout.Button("Add Missing Language") && EditorUtility.DisplayDialog("Add Missing Language", "This Will Update Existing Language Data", "ok", "cancel"))
            {
                textMeshProLocalization.AddMissingLanguages();
                UpdateUI();
                EditorUtility.SetDirty(textMeshProLocalization);
            }

            if (GUILayout.Button("Load Language Default Data") && EditorUtility.DisplayDialog("Load Language Default Data", "This Will Rewrite Existing Loaded Data", "ok", "cancel"))
            {
                textMeshProLocalization.RetrieveAllLanguages();
                UpdateUI();
                EditorUtility.SetDirty(textMeshProLocalization);
            }

            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}

