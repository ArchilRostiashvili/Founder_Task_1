using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using CustomEditorUtilities;
using UnityEditor.UIElements;
using System.Linq;

namespace BebiLibs.AudioSystem
{

    public class LocalizedAudioInspector : EditorWindow
    {

        [MenuItem("BebiLibs/Localized Audio Inspector")]
        private static void ShowWindow()
        {
            var window = GetWindow<LocalizedAudioInspector>();
            window.titleContent = new GUIContent("Localized Audio Inspector");
            window.Show();
            window.minSize = new Vector2(500f, 300f);
        }

        private VisualElement _settingsPanel;
        private VisualElement _contentPanel;
        private VisualElement _audioListPanel;
        private List<LocalizedAudioTrackSO> _locAudioList;

        private LocalizedAudioTrackInspectorConfig _config;
        //private int _fontSize = 10;

        private void OnEnable()
        {
            _config = ConfigCreator.LoadOrCreateConfig<LocalizedAudioTrackInspectorConfig>("LocalizedAudioTrackInspectorConfig", "Assets/Editor/EditorConfigs");
        }

        private void CreateGUI()
        {
            VisualElement mainPanel = new VisualElement();
            mainPanel.style.flexGrow = 1;
            mainPanel.SetFlexDirection(FlexDirection.Row);
            rootVisualElement.Add(mainPanel);

            TwoPaneSplitView splitView = new TwoPaneSplitView(0, 400, TwoPaneSplitViewOrientation.Horizontal);
            splitView.SetFlexGrow(4);

            _settingsPanel = new VisualElement();
            _settingsPanel.SetFlexGrow(1);
            _settingsPanel.SetFlexShrink(2);

            _contentPanel = new VisualElement();
            _audioListPanel = new VisualElement();

            mainPanel.Add(_settingsPanel);
            mainPanel.Add(UIElementUtility.SimpleLineVertical(new Color(0.3490196f, 0.3490196f, 0.3490196f), 1));
            mainPanel.Add(splitView);

            splitView.Add(_audioListPanel);
            splitView.Add(_contentPanel);

            UpdateSettingsPanel();
            UpdateAudioListPanel();
        }

        public void UpdateSettingsPanel()
        {
            _settingsPanel.Clear();
            _settingsPanel.style.maxWidth = 300;
            _settingsPanel.AddHeaderLabel("Settings");

            SerializedObject sObj = null;
            PropertyField audioGroupFiled = UIElementUtility.CreateProperty(_config, nameof(_config.AudioGroupFilter), ref sObj);
            UpdateListProperty(audioGroupFiled);

            PropertyField toggle = UIElementUtility.CreateProperty(_config, nameof(_config.CheckForLanguageMatch), ref sObj);

            PropertyField languageFilter = UIElementUtility.CreateProperty(_config, nameof(_config.LanguageFilter), ref sObj);
            UpdateListProperty(languageFilter);

            _settingsPanel.Add(audioGroupFiled);
            _settingsPanel.Add(toggle);
            _settingsPanel.Add(languageFilter);
            _settingsPanel.AddLine(out _);

            Button button = _settingsPanel.AddButton("Filter", () =>
            {
                UpdateAudioListPanel();
            });
            button.SetTextStyles(13, FontStyle.Bold);
            button.SetPadding(6, 6, 8, 8);
            button.SetMargin(5, 5, 5, 5);

            Button button1 = _settingsPanel.AddButton("Fix Localized Assets", () =>
            {
                FixLocalizedAsset();
            });
            button1.SetTextStyles(13, FontStyle.Bold);
            button1.SetPadding(6, 6, 8, 8);
            button1.SetMargin(5, 5, 0, 5);
        }

        private void FixLocalizedAsset()
        {
            LoadAudioTrackData();
            LocalizedTrackFixer.FixTracks(_locAudioList);
        }


        private void UpdateListProperty(PropertyField propertyField)
        {
            propertyField.SetMargin(5, 5, 5, 5);
            propertyField.name = "List Field";
            propertyField.style.maxHeight = 200;
        }

        private string FormatListItem(string arg) => arg;
        private string FormatSelectedValues(string arg) => arg;

        private void LoadAudioTrackData()
        {
            _locAudioList = UnityFileUtils.FindScriptableOBjectByType<LocalizedAudioTrackSO>();
            if (_config.AudioGroupFilter != null && _config.AudioGroupFilter.Count > 0)
            {
                _locAudioList = _locAudioList.Where(x => _config.AudioGroupFilter.Contains(x.AudioTrackGroup)).ToList();
            }

            if (_config.LanguageFilter != null && _config.LanguageFilter.Count > 0)
            {
                if (_config.CheckForLanguageMatch)
                {
                    _locAudioList = _locAudioList.Where(x => x.LocalizedTrackDataList.Any(z => _config.LanguageFilter.Contains(z.LanguageIdentifier))).ToList();
                }
                else
                {
                    _locAudioList = _locAudioList.Where(x => !x.LocalizedTrackDataList.Any(z => _config.LanguageFilter.Contains(z.LanguageIdentifier))).ToList();
                }
            }

        }

        private void UpdateAudioListPanel()
        {
            _audioListPanel.Clear();
            _audioListPanel.AddHeaderLabel("Localized Audio Track List");
            LoadAudioTrackData();
            ListView listView = new ListView(_locAudioList, -1, CreateLocalizedAudioTrackSOEntry, BindLocalizedAudioTrackSOEntry)
            {
                selectionType = SelectionType.Single
            };

            listView.onSelectionChange += (IEnumerable<object> objects) =>
            {
                UpdateContentPanel((LocalizedAudioTrackSO)objects.First());
            };

            _audioListPanel.Add(listView);
        }

        private void BindLocalizedAudioTrackSOEntry(VisualElement visualElementParent, int elementIndex)
        {
            Label label = (Label)visualElementParent;
            var element = _locAudioList[elementIndex];
            label.text = element.name;
        }

        private VisualElement CreateLocalizedAudioTrackSOEntry()
        {
            Label label = new Label();
            label.SetTextAlign(TextAnchor.MiddleLeft);
            label.SetFontStyles(FontStyle.Bold);
            label.SetPadding(5, 5, 0, 0);
            return label;
        }

        private void UpdateContentPanel(LocalizedAudioTrackSO element)
        {
            _contentPanel.Clear();
            _contentPanel.SetMargin(5, 5, 5, 0);

            VisualElement audioTrackData = UIElementUtility.CreateVisualElement(element);
            _contentPanel.Add(audioTrackData);
        }

    }

}
