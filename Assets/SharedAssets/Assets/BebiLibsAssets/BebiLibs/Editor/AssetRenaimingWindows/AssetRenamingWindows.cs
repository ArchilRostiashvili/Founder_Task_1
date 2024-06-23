using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using CustomEditorUtilities;
using UnityEditor.UIElements;
using System.Linq;

namespace BebiLibs.EditorExtensions.AssetRenaming
{
    public class AssetRenamingWindows : EditorWindow
    {
        private const string _EDITOR_NAME = "Asset Renaming Utility Window";

        private VisualElement _contentPanel;
        private ListView _selectedObjectsListView;


        private VisualElement _settingsPanel;
        private VisualElement _filterPanel;
        private VisualElement _fitterConfigPanel;

        private int _fontSize = 10;

        private static List<Object> _SelectedObjectsList = new List<Object>();
        private static AssetRenamingConfig _DefaultAssetRenamingConfig;
        private static System.Action _OnWindowCloseEvent;
        private static System.Action _OnApplyFilterEvent;

        public static void ShowWindow(List<Object> selectedObjects, AssetRenamingConfig assetRenamingConfig, System.Action onWindowClose = null, System.Action onApplyFilter = null)
        {
            _SelectedObjectsList = selectedObjects;
            _DefaultAssetRenamingConfig = assetRenamingConfig;
            _OnWindowCloseEvent = onWindowClose;
            _OnApplyFilterEvent = onApplyFilter;

            var Window = GetWindow<AssetRenamingWindows>();
            Window.titleContent = new GUIContent(_EDITOR_NAME);
            Window.Show();
            Window.minSize = new Vector2(500f, 300f);
        }


        private void OnEnable()
        {
            if (_DefaultAssetRenamingConfig == null)
            {
                _DefaultAssetRenamingConfig = AssetRenamingConfig.Create();
            }
        }

        private void OnDestroy()
        {
            //_OnWindowCloseEvent?.Invoke();
        }


        private void CreateGUI()
        {
            TwoPaneSplitView splitView = new TwoPaneSplitView(0, 300, TwoPaneSplitViewOrientation.Horizontal);
            splitView.SetFlexGrow(1);
            splitView.SetFlexDirection(FlexDirection.Row);

            int _contentPanelWidth = 40;
            _contentPanel = new VisualElement();
            _contentPanel.style.width = new StyleLength(new Length(_contentPanelWidth, LengthUnit.Percent));
            _settingsPanel = new VisualElement();
            _settingsPanel.style.width = new StyleLength(new Length(100 - _contentPanelWidth, LengthUnit.Percent));

            splitView.Add(_settingsPanel);
            // splitView.AddLine(LineDirection.Vertical, 2);
            splitView.Add(_contentPanel);

            rootVisualElement.Add(splitView);
            UpdateSettingsPanel();
            UpdateContentPanel();
        }


        private void UpdateSettingsPanel()
        {
            _settingsPanel.Clear();

            _settingsPanel.AddHeaderLabel("Formate Settings");
            _settingsPanel.SetID("SettingsPanel");

            _filterPanel = new VisualElement();
            _fitterConfigPanel = new VisualElement();

            _settingsPanel.Add(_filterPanel);
            _settingsPanel.AddLine(out _, LineDirection.Horizontal, 2);
            _settingsPanel.Add(_fitterConfigPanel);

            UpdateFiletedConfigPanel();
            UpdateFilterPanel();
        }

        private void UpdateFiletedConfigPanel()
        {
            VisualElement element = _DefaultAssetRenamingConfig.CreateVisualElement(5, () =>
            {
                UpdateFilterPanel();
            });

            element.SetID("InspectorElement");

            _fitterConfigPanel.SetMargin(0, 0, 15, 0);
            _fitterConfigPanel.Add(element);
        }

        private void UpdateFilterPanel()
        {
            _filterPanel.Clear();
            int indexOfSelectedPrefix = _DefaultAssetRenamingConfig.PrefixesList.IndexOf(_DefaultAssetRenamingConfig.SelectedPrefix);
            indexOfSelectedPrefix = indexOfSelectedPrefix == -1 ? 0 : indexOfSelectedPrefix;
            PopupField<string> prefixPopup = new PopupField<string>("Selected Prefix ", _DefaultAssetRenamingConfig.PrefixesList, indexOfSelectedPrefix);
            prefixPopup.RegisterValueChangedCallback((evt) =>
            {
                _DefaultAssetRenamingConfig.SelectedPrefix = evt.newValue;
            });

            int indexOfSelectedSuffix = _DefaultAssetRenamingConfig.SuffixList.IndexOf(_DefaultAssetRenamingConfig.SelectedSuffix);
            indexOfSelectedSuffix = indexOfSelectedSuffix == -1 ? 0 : indexOfSelectedSuffix;
            PopupField<string> suffixPopup = new PopupField<string>("Selected Suffix ", _DefaultAssetRenamingConfig.SuffixList, indexOfSelectedSuffix);
            suffixPopup.RegisterValueChangedCallback((evt) =>
            {
                _DefaultAssetRenamingConfig.SelectedSuffix = evt.newValue;
            });

            Toggle toggle = new Toggle("LowerCase");
            toggle.value = _DefaultAssetRenamingConfig.LowerCase;
            toggle.RegisterValueChangedCallback((evt) =>
            {
                _DefaultAssetRenamingConfig.LowerCase = evt.newValue;
            });


            _filterPanel.Add(prefixPopup);
            _filterPanel.Add(suffixPopup);
            _filterPanel.Add(toggle);


            Button previewButton = _filterPanel.AddButton("Preview Filter", () =>
            {
                RegenerateSelectedObjectListView();
            });

            previewButton.SetTextStyles(_fontSize + 4, FontStyle.Bold);
            previewButton.SetPadding(6, 6, 7, 7);

            Button apply = _filterPanel.AddButton("Apply Filter", () =>
            {
                if (!EditorUtility.DisplayDialog("Apply Filter", "Are you sure you want to apply filter?", "Yes", "No"))
                    return;
                AssetRenameUtility.ModifyAssetName(_SelectedObjectsList, _DefaultAssetRenamingConfig);
                Close();
                _OnApplyFilterEvent?.Invoke();
            });

            apply.SetTextStyles(_fontSize + 4, FontStyle.Bold);
            apply.SetPadding(6, 6, 7, 7);

            Button cancel = _filterPanel.AddButton("Continue Without Filter", () =>
            {
                Close();
                _OnWindowCloseEvent?.Invoke();
            });
            cancel.SetTextStyles(_fontSize + 4, FontStyle.Bold);
            cancel.SetPadding(6, 6, 7, 7);

            _filterPanel.SetMargin(0, 0, 0, 7);
        }



        private void UpdateContentPanel()
        {
            _contentPanel.Clear();
            _contentPanel.SetID("ContentPanel");
            _contentPanel.AddHeaderLabel("Preprocessing Preview");

            VisualElement listElement = new VisualElement();
            listElement.SetFlexGrow(1);

            VisualElement header = new VisualElement();
            header.SetFlexDirection(FlexDirection.Row);

            TextElement oldName = new TextElement();
            oldName.text = "Old Name";
            //oldName.SetTextAlign(TextAnchor.MiddleCenter);
            oldName.SetMargin(10, 0, 10, 10).SetTextStyles(_fontSize + 3, FontStyle.Bold);
            oldName.style.width = new StyleLength(new Length(50, LengthUnit.Percent));

            TextElement newName = new TextElement();
            newName.text = "New Name";
            //newName.SetTextAlign(TextAnchor.MiddleCenter);
            newName.SetMargin(0, 0, 10, 10).SetTextStyles(_fontSize + 3, FontStyle.Bold);
            oldName.style.width = new StyleLength(new Length(50, LengthUnit.Percent));

            header.Add(oldName);
            header.Add(newName);
            listElement.Add(header);

            listElement.AddLine(out _, LineDirection.Horizontal, 2);

            _selectedObjectsListView = new ListView(_SelectedObjectsList, -1, MakeObjectItem, BindObjectItem).SetMargin(5);
            listElement.Add(_selectedObjectsListView);

            listElement.AddLine(out VisualElement line, LineDirection.Vertical, 1);
            line.style.position = Position.Absolute;
            line.SetID("Line");
            line.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            line.style.left = new StyleLength(new Length(50, LengthUnit.Percent));



            _contentPanel.Add(listElement);
        }

        public void RegenerateSelectedObjectListView()
        {
            _selectedObjectsListView.Rebuild();
        }

        private void BindObjectItem(VisualElement listViewElement, int index)
        {
            TextElement oldName = listViewElement.Q<TextElement>("old_name_field");
            oldName.text = _SelectedObjectsList[index].name;

            TextElement newName = listViewElement.Q<TextElement>("new_name_field");
            newName.text = AssetRenameUtility.ApplyFilter(_SelectedObjectsList[index].name, _DefaultAssetRenamingConfig);
        }

        private VisualElement MakeObjectItem()
        {
            VisualElement visualElement = new VisualElement();
            visualElement.SetFlexDirection(FlexDirection.Row);
            visualElement.SetAlignItems(Align.Center);

            visualElement.SetMargin(2, 2, 0, 0);
            visualElement.style.height = 20;

            TextElement oldName = new TextElement();
            oldName.SetID("old_name_field");
            oldName.SetMargin(5, 0, 0, 0).SetTextStyles(_fontSize + 2, FontStyle.Bold).SetTextColor(Color.red);
            oldName.style.width = new StyleLength(new Length(50, LengthUnit.Percent));

            TextElement newName = new TextElement();
            newName.SetID("new_name_field");
            newName.SetMargin(5, 0, 0, 0).SetTextStyles(_fontSize + 2, FontStyle.Bold).SetTextColor(Color.green);
            oldName.style.width = new StyleLength(new Length(50, LengthUnit.Percent));

            visualElement.Add(oldName);
            visualElement.Add(newName);
            return visualElement;

        }

    }
}
