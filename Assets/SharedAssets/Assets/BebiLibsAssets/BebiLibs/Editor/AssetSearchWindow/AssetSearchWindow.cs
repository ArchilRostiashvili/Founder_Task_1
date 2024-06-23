using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UIElements;
    using CustomEditorUtilities;
    using UnityEditor.UIElements;

    public class AssetSearchWindow : EditorWindow
    {
        private const string EditorName = "Asset Search Window";

        private static string _AssetType = "prefab";
        private static System.Type _TypeToSearch = typeof(PopUpBase);
        private static string _EditorDescription = "Select a prefab";
        private static System.Action<Object> _OnItemSelectEvent;
        private static System.Action _OnWindowCloseEvent;

        private VisualElement _contentPanel;
        private VisualElement _resultPanel;


        public static void ShowWindow(string editorDescription, string assetType, System.Type typeToSearch, System.Action<UnityEngine.Object> onItemSelect, System.Action onWindowClose)
        {
            _AssetType = assetType;
            _TypeToSearch = typeToSearch;
            _EditorDescription = editorDescription;
            _OnItemSelectEvent = onItemSelect;
            _OnWindowCloseEvent = onWindowClose;

            var window = GetWindow<AssetSearchWindow>();
            window.titleContent = new GUIContent(EditorName);
            window.Show();
            window.minSize = new Vector2(300f, 400f);
        }

        List<Object> _objects = new List<Object>();

        private void OnEnable()
        {
            FindPrefabsInProjectWithBaseType(_AssetType, _TypeToSearch);
        }

        private void OnDestroy()
        {
            _OnWindowCloseEvent?.Invoke();
        }

        private void FindPrefabsInProjectWithBaseType(string assetType, System.Type type)
        {
            _objects.Clear();
            string[] guids = AssetDatabase.FindAssets($"t:{assetType}", new[] { "Assets" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (assetType == "prefab")
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab.GetComponent(type) != null)
                    {
                        _objects.Add(prefab);
                    }
                }
                else
                {
                    var obj = AssetDatabase.LoadAssetAtPath(path, type);
                    if (obj != null)
                    {
                        _objects.Add(obj);
                    }
                }
            }
        }

        private void CreateGUI()
        {
            VisualElement mainPanel = new VisualElement();
            mainPanel.style.flexGrow = 1;
            rootVisualElement.Add(mainPanel);

            _contentPanel = new VisualElement();

            mainPanel.Add(_contentPanel);
            UpdateContentPanel();
        }

        private void UpdateContentPanel()
        {
            _contentPanel.Clear();

            _contentPanel.AddHeaderLabel(EditorName);

            _contentPanel.Add(UIElementUtility.SimpleLineHorizontal(new Color32(89, 89, 89, 255), 1));
            if (!string.IsNullOrEmpty(_EditorDescription))
            {
                TextElement textElement = new TextElement();
                textElement.SetMargin(5);
                textElement.text = _EditorDescription;
                textElement.SetTextStyles(13, FontStyle.Bold);
                _contentPanel.Add(textElement);
            }

            ListView listView = new ListView(_objects, -1, MakeObjectItem, BindObjectItem).SetMargin(5);

            _contentPanel.Add(listView);
            _contentPanel.Add(UIElementUtility.SimpleLineHorizontal(new Color32(89, 89, 89, 255), 1));

            VisualElement buttonParent = new VisualElement()
                .SetFlexDirection(FlexDirection.Row)
                .SetJustifyContent(Justify.Center);
            buttonParent.style.height = 37;
            buttonParent.style.minHeight = 37;
            buttonParent.SetFlexShrink(1);

            Button buttonSelect = buttonParent.AddButton("Select", () =>
            {
                _OnItemSelectEvent?.Invoke(_objects[listView.selectedIndex]);
                Close();
            }).SetMargin(5).SetFlexGrow(1);

            Button buttonClose = buttonParent.AddButton("Close", () =>
            {
                _OnWindowCloseEvent?.Invoke();
                Close();
            }).SetMargin(5).SetFlexGrow(1);

            _contentPanel.Add(buttonParent);
            _contentPanel.Add(_resultPanel);
        }

        private void BindObjectItem(VisualElement arg1, int arg2)
        {
            ObjectField bindObject = arg1.Q<ObjectField>("object_name_field");
            bindObject.label = _objects[arg2].name;
            bindObject.value = _objects[arg2];

            Label objLabel = bindObject.Query<Label>(null, "unity-object-field__label");
            objLabel.style.maxWidth = 200;
            objLabel.style.minWidth = new StyleLength(new Length(30, LengthUnit.Percent));
            objLabel.style.overflow = Overflow.Hidden;

        }

        private VisualElement MakeObjectItem()
        {
            VisualElement visualElement = new VisualElement();
            visualElement.SetFlexDirection(FlexDirection.Row);
            visualElement.SetAlignItems(Align.Center);
            visualElement.SetMargin(2, 2, 0, 0);
            visualElement.style.height = 20;

            ObjectField objectField = new ObjectField();
            objectField.SetFlexGrow(1);
            objectField.SetFlexShrink(1);
            objectField.name = "object_name_field";


            visualElement.Add(objectField);
            return visualElement;

        }
    }
}
