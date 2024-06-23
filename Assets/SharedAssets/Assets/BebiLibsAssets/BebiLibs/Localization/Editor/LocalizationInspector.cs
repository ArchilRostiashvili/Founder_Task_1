using BebiLibs.Localization.Core;
using CustomEditorUtilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BebiLibs.Localization.LocalizationEditor
{
    public class LocalizationInspector : EditorWindow
    {
        static LocalizationInspector window;
        [MenuItem("BebiLibs/Localization Editor")]
        private static void ShowWindow()
        {
            window = GetWindow<LocalizationInspector>();
            window.titleContent = new GUIContent("Localization Editor");
            window.Show();
            window.minSize = new Vector2(600f, 300f);
        }

        private VisualElement _contentPanel;
        private VisualElement _audioListPanel;
        private List<LanguageIdentifier> _locAudioList;
        private LanguageIdentifier _activeLanguage;

        //private int _fontSize = 10;


        private void CreateGUI()
        {
            VisualElement mainPanel = new VisualElement();
            mainPanel.style.flexGrow = 1;
            mainPanel.SetFlexDirection(FlexDirection.Row);
            rootVisualElement.Add(mainPanel);

            TwoPaneSplitView splitView = new TwoPaneSplitView(0, 300, TwoPaneSplitViewOrientation.Horizontal);
            splitView.SetFlexGrow(4);


            _contentPanel = new VisualElement();
            _audioListPanel = new VisualElement();

            mainPanel.Add(UIElementUtility.SimpleLineVertical(new Color(0.3490196f, 0.3490196f, 0.3490196f), 1));
            mainPanel.Add(splitView);

            splitView.Add(_audioListPanel);
            splitView.Add(_contentPanel);
            UpdateAudioListPanel();
        }

        private void OnEnable()
        {
            _activeLanguage = LocalizationManager.ActiveLanguage;
        }

        private void UpdateListProperty(PropertyField propertyField)
        {
            propertyField.SetMargin(5, 5, 5, 5);
            propertyField.name = "List Field";
            propertyField.style.maxHeight = 200;
        }

        private string FormatListItem(string arg) => arg;
        private string FormatSelectedValues(string arg) => arg;

        private void UpdateAudioListPanel()
        {
            _audioListPanel.Clear();
            _audioListPanel.AddHeaderLabel("Localized Language Inspector");
            _locAudioList = UnityFileUtils.FindScriptableOBjectByType<LanguageIdentifier>();

            Button addNewItemButton = _audioListPanel.AddButton("Add new language", () =>
            {
                TextInputWindow.ShowWindow("Insert Language Name", string.Empty, (string input) =>
                {
                    if (string.IsNullOrEmpty(input))
                    {
                        EditorUtility.DisplayDialog("Error", "Name cannot be empty", "Ok");
                        return;
                    }

                    if (_locAudioList.Any(x => x.name == input))
                    {
                        EditorUtility.DisplayDialog("Error", "Name already exists", "Ok");
                        return;
                    }

                    string defaultFolder = "Assets";

                    if (_locAudioList.Count > 0)
                    {
                        defaultFolder = AssetDatabase.GetAssetPath(_locAudioList[0]);
                        defaultFolder = Path.GetDirectoryName(defaultFolder);
                    }

                    EditorUtility.SaveFolderPanel("Select folder", defaultFolder, string.Empty);
                    LanguageIdentifier identifier = ConfigCreator.LoadOrCreateConfig<LanguageIdentifier>(input, defaultFolder);
                    _locAudioList.Add(identifier);
                    UpdateAudioListPanel();
                });
            });

            addNewItemButton.SetTextStyles(15, FontStyle.Bold).SetMargin(5, 5, 8, 2);

            Button UpdatePanelButton = _audioListPanel.AddButton("Update List", () =>
            {
                UpdateAudioListPanel();
            });
            UpdatePanelButton.SetTextStyles(15, FontStyle.Bold).SetMargin(5, 5, 2, 8);



            _locAudioList = UnityFileUtils.FindScriptableOBjectByType<LanguageIdentifier>();

            ListView listView = new ListView(_locAudioList, -1, CreateLocalizedAudioTrackSOEntry, BindLocalizedAudioTrackSOEntry)
            {
                selectionType = SelectionType.Single
            };


            listView.onSelectionChange += (IEnumerable<object> objects) =>
                     {
                         UpdateContentPanel((LanguageIdentifier)objects.First());
                     };

            _audioListPanel.AddLine(out VisualElement line);
            _audioListPanel.Add(listView);
        }

        private void BindLocalizedAudioTrackSOEntry(VisualElement visualElementParent, int elementIndex)
        {
            Label label = (Label)visualElementParent;
            LanguageIdentifier element = _locAudioList[elementIndex];

            if (element == _activeLanguage)
            {
                label.SetTextColor(Color.green);
                label.text = element.name + " (Active)";
            }
            else
            {
                label.SetTextColor(new Color(0.8235294f, 0.8235294f, 0.8235294f));
                label.text = element.name;
            }
        }

        private VisualElement CreateLocalizedAudioTrackSOEntry()
        {
            Label label = new Label();
            label.SetTextAlign(TextAnchor.MiddleLeft);
            label.SetFontStyles(FontStyle.Bold);
            label.SetPadding(5, 5, 0, 0);
            return label;
        }

        private void UpdateContentPanel(LanguageIdentifier element)
        {
            _contentPanel.Clear();
            _contentPanel.SetMargin(5, 5, 5, 0);

            VisualElement audioTrackData = UIElementUtility.CreateVisualElement(element);
            _contentPanel.Add(audioTrackData);
            audioTrackData.SetMargin(0, 0, 0, 8);

            _contentPanel.AddLine(out VisualElement line);

            Button button = _contentPanel.AddButton("Set As Active Language", () =>
            {
                LocalizationManager.SetActiveLanguage(element);
                _activeLanguage = element;
                UpdateAudioListPanel();
            });
            button.SetTextStyles(15, FontStyle.Bold).SetMargin(5, 5, 8, 2);

            Button selectButton = _contentPanel.AddButton("Ping Object", () =>
            {
                EditorGUIUtility.PingObject(element);
            });
            selectButton.SetTextStyles(15, FontStyle.Bold).SetMargin(5, 5, 2, 8);
        }

    }
}
