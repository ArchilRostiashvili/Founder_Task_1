using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using System;

namespace BebiLibs.ModulesGameSystem.ModuleEditor
{
    public class ModuleInspector : EditorWindow
    {
        [MenuItem("BebiLibs/ModuleInspector", false, 17)]
        private static void ShowWindow()
        {
            var window = GetWindow<ModuleInspector>();
            window.titleContent = new GUIContent("ModuleInspector");
            window.Show();
            window.minSize = new Vector2(400f, 200f);
        }

        private List<ModuleData> _moduleDataList = new List<ModuleData>();
        private ModuleData _selectedModuleData;
        private VisualElement _rightDataPanel;

        private VisualElement _moduleDataPanel;
        private VisualElement _customDataDisplayPlane;

        private void OnEnable()
        {
            LoadModuleData();
        }

        private void LoadModuleData()
        {
            _moduleDataList.Clear();
            string[] guids = AssetDatabase.FindAssets("t:ModuleData");
            foreach (var guid in guids)
            {
                string moduleAssetPath = AssetDatabase.GUIDToAssetPath(guid);
                ModuleData moduleData = AssetDatabase.LoadAssetAtPath<ModuleData>(moduleAssetPath);
                if (moduleData != null)
                {
                    _moduleDataList.Add(moduleData);
                }
            }
        }

        public void CreateGUI()
        {
            LoadModuleData();

            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(splitView);

            VisualElement leftPane = GetModuleListPanel();
            splitView.Add(leftPane);
            _rightDataPanel = new VisualElement();
            splitView.Add(_rightDataPanel);
        }

        private VisualElement GetModuleListPanel()
        {
            var leftPane = new ListView();
            leftPane.makeItem = () => new Label();
            leftPane.bindItem = (item, index) =>
            {
                Label label = item as Label;
                label.style.paddingLeft = 5;
                label.style.unityFontStyleAndWeight = FontStyle.Bold;
                label.style.unityTextAlign = TextAnchor.MiddleLeft;
                label.text = _moduleDataList[index].name;
            };

            leftPane.itemsSource = _moduleDataList;

            leftPane.onSelectionChange += OnModuleSelectionChange;
            return leftPane;
        }


        private void OnModuleSelectionChange(IEnumerable<object> selectedItems)
        {
            _rightDataPanel.Clear();

            _moduleDataPanel = new ScrollView();
            _customDataDisplayPlane = new ScrollView();

            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            _rightDataPanel.Add(splitView);

            var moduleData = selectedItems.First() as ModuleData;
            if (moduleData == null)
                return;

            UpdateCustomDataVisualElement(_customDataDisplayPlane, moduleData);
            _moduleDataPanel.Add(CreateUIElementInspector(moduleData));

            splitView.Add(_customDataDisplayPlane);
            splitView.Add(_moduleDataPanel);
        }

        private void UpdateCustomDataVisualElement(VisualElement visualElement, ModuleData moduleData)
        {
            visualElement.Add(CreateButton("Disable Scene From Build Settings", () => ModuleSceneHelper.SetModuleSceneEnabledState(moduleData, false)));
            visualElement.Add(CreateButton("Enable Scene From Build Settings", () => ModuleSceneHelper.SetModuleSceneEnabledState(moduleData, true)));
            visualElement.Add(SimpleLine());
            visualElement.Add(GetModuleGameListPanel(moduleData));
        }

        public VisualElement SimpleLine()
        {
            VisualElement line = new VisualElement();
            line.style.borderTopColor = new StyleColor(new Color(120, 120, 120));
            line.style.borderTopWidth = 1f;
            line.style.marginTop = 5;
            return line;
        }

        private VisualElement GetModuleGameListPanel(ModuleData moduleData)
        {
            var leftPane = new ListView();
            leftPane.makeItem = () => new Label();
            leftPane.bindItem = (item, index) =>
            {
                Label label = item as Label;
                label.style.paddingLeft = 5;
                label.style.unityFontStyleAndWeight = FontStyle.Bold;
                label.style.unityTextAlign = TextAnchor.MiddleLeft;
                label.text = moduleData.ModuleGameDataList[index].name;
            };

            leftPane.itemsSource = moduleData.ModuleGameDataList;
            leftPane.onSelectionChange += OnGameSelectionChange;
            return leftPane;
        }

        private void OnGameSelectionChange(IEnumerable<object> selectedItems)
        {
            _moduleDataPanel.Clear();
            var moduleGameData = selectedItems.First() as ModuleGameData;
            _moduleDataPanel.Add(CreateUIElementInspector(moduleGameData));
        }



        private VisualElement CreateButton(string text, Action action)
        {
            Button button = new Button(action);
            button.text = text;

            return button;
        }

        private VisualElement CreateUIElementInspector(UnityEngine.Object target)
        {
            Editor editor = Editor.CreateEditor(target);
            IMGUIContainer inspectorIMGUI = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
            inspectorIMGUI.style.marginBottom = 5;
            inspectorIMGUI.style.marginLeft = 5;
            inspectorIMGUI.style.marginRight = 5;
            inspectorIMGUI.style.marginTop = 5;
            return inspectorIMGUI;
        }
    }
}
