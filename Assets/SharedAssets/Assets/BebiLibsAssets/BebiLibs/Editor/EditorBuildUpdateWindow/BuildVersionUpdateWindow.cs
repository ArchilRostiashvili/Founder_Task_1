using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using System;

namespace CustomEditorBuildWindow
{
    public class BuildVersionUpdateWindow : EditorWindow
    {
        private Label _version_name_label;
        private TextField _version_number_field;
        private IntegerField _build_number_field;
        private Button _version_apply_button;
        private Toggle _increment_build_version_toggle;
        private Toggle _rename_build_version_toggle;
        private Button _cancel_button;
        private Button _build_button;
        private BuildVersionUpdateConfig config;
        private GameBuildVersionData versionData;
        private static BuildPlayerOptions _BuildPlayerOptions;
        private static bool _IsBuildOptionSet;

        private static EditorWindow window;

        public bool IsBuildVersionIncremented
        {
            get => EditorPrefs.GetBool("is_build_version_incremented", false);
            set => EditorPrefs.SetBool("is_build_version_incremented", value);
        }

        [MenuItem("File/Build", false, 205)]
        private static void ShowWindow()
        {
            if (window != null)
            {
                window.Close();
                Debug.Log("Close Old Window");
            }

            ShowWindow(EditorBuildHelper.GetDefaultBuildPlayerOptions());
        }

        public static void ShowWindow(BuildPlayerOptions buildPlayerOptions)
        {
            _BuildPlayerOptions = buildPlayerOptions;
            _IsBuildOptionSet = true;
            window = GetWindow<BuildVersionUpdateWindow>();
            window.titleContent = new GUIContent("Build Version Updater");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            StyleSheet styleSheet = FindAndLoadAssetByName<StyleSheet>("BuildVersionUpdateWindow");
            VisualTreeAsset visualTree = FindAndLoadAssetByName<VisualTreeAsset>("BuildVersionUpdateWindow");

            VisualElement editorBuilderRoot = visualTree.Instantiate();
            editorBuilderRoot.style.flexGrow = 1;

            root.Add(editorBuilderRoot);
            root.styleSheets.Add(styleSheet);

            GetUIComponents();
            UpdateUIComponents();
        }

        public void GetUIComponents()
        {
            VisualElement root = rootVisualElement;
            _version_name_label = root.Q<Label>("version_name_label");
            _version_number_field = root.Q<TextField>("version_number_field");
            _build_number_field = root.Q<IntegerField>("build_number_field");
            _version_apply_button = root.Q<Button>("version_apply_button");
            _increment_build_version_toggle = root.Q<Toggle>("increment_build_version_toggle");
            _rename_build_version_toggle = root.Q<Toggle>("rename_build_version_toggle");
            _cancel_button = root.Q<Button>("cancel_button");
            _build_button = root.Q<Button>("build_button");
        }

        public void UpdateUIComponents()
        {
            InitializeConfig();

            _version_name_label.text = versionData.BuildVersionName;
            _version_number_field.BindProperty(config.GetVersionStringProperty(versionData));
            _version_number_field.RegisterValueChangedCallback(OnVersionNumberChanged);

            _build_number_field.BindProperty(config.GetBuildNumberProperty(versionData));
            _build_number_field.maxLength = 6;
            _build_number_field.RegisterValueChangedCallback(OnBuildNumberChanged);

            _increment_build_version_toggle.BindProperty(config.GetIncrementBuildVersionProperty());
            _rename_build_version_toggle.BindProperty(config.GetRenameBuildVersionProperty());

            _version_apply_button.clicked += OnApplyButtonClicked;
            _cancel_button.clicked += OnCloseButtonClick;
            _build_button.clicked += OnBuildButtonClick;

            IncrementBuildVersion();
            UpdatePlayButtonState();
        }

        private void InitializeConfig()
        {
            config = BuildVersionConfigProvider.FindOrCreateAsset();
            versionData = config.GetActiveBuildVersion();
        }

        public void IncrementBuildVersion()
        {
            if (config.IncrementBuildVersion && !IsBuildVersionIncremented)
            {
                versionData.BuildNumber++;
                config.Save();
                IsBuildVersionIncremented = true;
            }
        }

        public void UpdatePlayButtonState()
        {
            bool isBuildVersionUpdated = config.IsProjectSettingBuildVersionUpdated();
            _version_apply_button.SetEnabled(!isBuildVersionUpdated);
            _version_apply_button.text = isBuildVersionUpdated ? "Project Setting are already Updated" : "Apply Changes To Project Settings";
        }

        private void OnBuildNumberChanged(ChangeEvent<int> evt)
        {
            _build_number_field.SetValueWithoutNotify(GameBuildVersionData.ValidateBuildNumber(evt.newValue));
            UpdatePlayButtonState();
        }

        private void OnVersionNumberChanged(ChangeEvent<string> evt)
        {
            _version_number_field.SetValueWithoutNotify(GameBuildVersionData.ValidateVersionString(evt.newValue));
            UpdatePlayButtonState();
        }

        private void OnApplyButtonClicked()
        {
            config.SetProjectSettingBuildVersion(versionData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UpdatePlayButtonState();
        }

        private void OnBuildButtonClick()
        {
            if (!_IsBuildOptionSet)
            {
                _BuildPlayerOptions = EditorBuildHelper.GetDefaultBuildPlayerOptions();
            }

            if (config.RenameBuildVersion)
            {
                string newBuildPath = EditorBuildHelper.RenameBuildName(_BuildPlayerOptions.locationPathName, versionData.FullVersionString);
                _BuildPlayerOptions.locationPathName = newBuildPath;
            }

            Close();
            IsBuildVersionIncremented = false;
            EditorBuildHelper.BuildPlayer(_BuildPlayerOptions);
        }

        private void OnCloseButtonClick()
        {
            Close();
        }

        private static T FindAndLoadAssetByName<T>(string name) where T : UnityEngine.Object
        {
            string[] guids = AssetDatabase.FindAssets(name + " t:" + typeof(T).Name);
            if (guids.Length == 0)
            {
                Debug.LogError("Could not find asset with name: " + name);
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
}
