using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using BebiLibs;

namespace BebiLibs.Localization.LocalizationEditor
{
    public class TextMeshProLocalizationEditorViewer : EditorWindow
    {

        [MenuItem("BigToddlers/TextMeshProLocalizationViewer")]
        private static void ShowWindow()
        {
            var window = GetWindow<TextMeshProLocalizationEditorViewer>();
            window.titleContent = new GUIContent("TextMeshProLocalizationViewer");
            window.Show();
        }

        public Editor tmpEditor;
        GameObject LastActivegameObject;
        TextMeshProLocalization textMeshProLocalization;
        SerializedObject _tmpObject;

        private void OnInspectorUpdate()
        {
            this.UpdateSelection();
            if (_tmpObject != null)
            {
                _tmpObject.UpdateIfRequiredOrScript();
                _tmpObject.ApplyModifiedProperties();
            }
        }

        private void OnSelectionChange()
        {
            this.UpdateSelection();
            this.Repaint();
        }

        private void OnGUI()
        {
            this.UpdateView();
        }

        public void UpdateSelection()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject != this.LastActivegameObject)
            {
                this.LastActivegameObject = Selection.activeGameObject;
                this.textMeshProLocalization = Selection.activeGameObject.GetComponent<TextMeshProLocalization>();
                if (this.textMeshProLocalization != null)
                {
                    _tmpObject = new SerializedObject(this.textMeshProLocalization);
                    this.tmpEditor = Editor.CreateEditor(this.textMeshProLocalization);
                }
                else
                {
                    _tmpObject = null;
                    this.tmpEditor = null;
                }
            }
        }

        private void UpdateView()
        {
            if (this.textMeshProLocalization != null)
            {
                EditorGUILayout.LabelField(this.textMeshProLocalization.name);
            }

            if (this.tmpEditor != null)
            {
                this.tmpEditor.OnInspectorGUI();
            }
        }


    }
#endif
}
