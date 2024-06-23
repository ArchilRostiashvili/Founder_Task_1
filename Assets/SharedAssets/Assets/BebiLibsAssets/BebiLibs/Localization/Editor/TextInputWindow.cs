using CustomEditorUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BebiLibs.Localization.LocalizationEditor
{
    public class TextInputWindow : EditorWindow
    {
        private static TextInputWindow _Window;
        private static System.Action<string> _OnProjectCloseEvent;
        private static string _DefaultInput;
        private static string _PopupName;

        public static void ShowWindow(string popupName, string defaultInput, System.Action<string> onProjectCloseEvent)
        {
            _OnProjectCloseEvent = onProjectCloseEvent;
            _DefaultInput = defaultInput;
            _PopupName = popupName;

            _Window = GetWindow<TextInputWindow>();
            _Window.titleContent = new GUIContent("Localization Editor");

            var position = _Window.position;
            position.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
            _Window.position = position;

            _Window.minSize = new Vector2(306f, 189f);
            _Window.maxSize = new Vector2(307f, 190f);

            _Window.Show();
        }

        public string EditorWindowText = "Choose a project name: ";
        private string _newProjectName;
        private VisualElement _contentPanel;

        private void CreateGUI()
        {
            VisualElement mainPanel = new VisualElement();
            rootVisualElement.Add(mainPanel);

            _contentPanel = new VisualElement();
            UpdateContentPanel();
            mainPanel.Add(_contentPanel);
        }

        private void UpdateContentPanel()
        {
            _contentPanel.Clear();
            _contentPanel.AddHeaderLabel(_PopupName, 10, 10);


            TextField textField = new TextField()
            .SetMargin(10, 10, 10, 5)
            .SetPadding(2, 2, 5, 5)
            .SetTextAlign(TextAnchor.MiddleLeft)
            .SetTextStyles(21, FontStyle.Normal);

            textField.value = _DefaultInput;

            _contentPanel.Add(textField);

            VisualElement buttonContainer = new VisualElement();
            buttonContainer.SetFlexDirection(FlexDirection.Row);
            buttonContainer.SetFlexGrow(1).SetJustifyContent(Justify.SpaceAround);
            buttonContainer.SetMargin(10, 10, 5, 0);


            buttonContainer.AddButton("OK", () =>
            {
                _OnProjectCloseEvent?.Invoke(textField.value);
                Close();
            }).SetFlexGrow(1).SetPadding(2, 2, 5, 5).SetTextStyles(20, FontStyle.Normal);


            buttonContainer.AddButton("Close", () =>
            {
                Close();
            }).SetFlexGrow(1).SetPadding(2, 2, 5, 5).SetTextStyles(20, FontStyle.Normal);

            _contentPanel.Add(buttonContainer);

        }
    }
}
