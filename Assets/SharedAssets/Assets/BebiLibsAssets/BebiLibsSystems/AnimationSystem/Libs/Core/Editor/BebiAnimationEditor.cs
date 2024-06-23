using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;


namespace BebiAnimations.Libs.Core
{
    [CustomEditor(typeof(BebiAnimation))]
    public class BebiAnimationEditor : Editor
    {
        internal static readonly string _UndoAdd = "Add Element To Array";
        internal static readonly string _UndoRemove = "Remove Element From Array";
        internal static readonly string _UndoMove = "Reorder Element In Array";

        private BebiAnimation _actionInvoker;

        private List<System.Type> _actionTypesList = new List<System.Type>();
        private string[] _optionsArray = new string[0];

        private ReorderableList _reorderableList;
        private SerializedProperty _actionArrayProperty;

        private Editor _objectEditor;

        private void OnEnable()
        {
            _actionInvoker = (BebiAnimation)target;
            _actionTypesList = GetActionSubtypes();

            _optionsArray = new string[_actionTypesList.Count];
            for (int i = 0; i < _optionsArray.Length; i++)
            {
                string name = _actionTypesList[i].Name;
                string fullName = _actionTypesList[i].FullName;
                string[] fullNameArray = fullName.Split('.');

                string newName = fullNameArray.Length > 3 ? string.Join('/', new List<string> { fullNameArray[^2], fullNameArray[^1] }) : name;
                _optionsArray[i] = newName;
            }

            _actionArrayProperty = serializedObject.FindProperty("_actionsList");

            _reorderableList = new ReorderableList(serializedObject, _actionArrayProperty, true, false, true, true)
            {
                drawElementCallback = DrawElementCallBack,
                elementHeightCallback = GetElementHeight,
                drawHeaderCallback = OnHeaderCallBack,
                onAddDropdownCallback = OnItemAddWithDropDown,
                onRemoveCallback = OnItemRemoveCallback,
                onCanRemoveCallback = OnCanRemoveItem,
                onSelectCallback = OnSelectCallBack
            };
            _objectEditor = null;
        }

        private void OnSelectCallBack(ReorderableList list)
        {
            _objectEditor = null;
            if (list.index < 0 && list.index >= list.count) return;

            SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(list.index);

            if (element == null && element.objectReferenceValue == null) return;
            _objectEditor = Editor.CreateEditor(element.objectReferenceValue);
        }

        private void OnItemAddWithDropDown(Rect buttonRect, ReorderableList list)
        {
            OnNewActionAddCallBack();
        }

        private bool OnCanRemoveItem(ReorderableList list)
        {
            return list.count > 0;
        }

        private void OnItemRemoveCallback(ReorderableList list)
        {
            int selectedIndex = list.index;
            if (selectedIndex >= list.count) return;

            if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete the Action?", "Yes", "No"))
            {

                SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(selectedIndex);
                if (element.objectReferenceValue != null)
                {
                    AnimationAction action = (AnimationAction)element.objectReferenceValue;
                    Undo.DestroyObjectImmediate(action.gameObject);
                }

                ReorderableList.defaultBehaviours.DoRemoveButton(list);

                serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }
            _objectEditor = null;
        }

        private void OnItemAdd(ReorderableList list)
        {
            Debug.Log("Element Added");
        }

        private void OnHeaderCallBack(Rect rect)
        {
            EditorGUI.LabelField(rect, "Action List");
        }

        private float GetElementHeight(int index)
        {
            if (index < 0 || index >= _reorderableList.count) return 0;

            SerializedProperty actionProperty = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            if (actionProperty != null && actionProperty.objectReferenceValue != null)
            {
                return EditorGUI.GetPropertyHeight(actionProperty) + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                return 0;
            }
        }

        private void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index < 0 || index >= _reorderableList.count) return;

            SerializedProperty actionProperty = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);

            if (actionProperty != null && actionProperty.objectReferenceValue != null)
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(rect, actionProperty, new GUIContent(actionProperty.objectReferenceValue.name));
                EditorGUI.indentLevel--;
            }
            else
            {
                RemoveNullElement(index);
            }
        }

        private void RemoveNullElement(int index)
        {
            if (index < 0 || index >= _reorderableList.count) return;
            _reorderableList.index = index;
            ReorderableList.defaultBehaviours.DoRemoveButton(_reorderableList);
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            DrawPropertiesExcluding(serializedObject, "_actionsList", "m_Script");

            _actionArrayProperty.isExpanded = EditorGUILayout.Foldout(_actionArrayProperty.isExpanded, _actionArrayProperty.displayName);
            if (_actionArrayProperty.isExpanded)
            {
                _reorderableList.DoLayoutList();
            }

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Play"))
                {
                    _actionInvoker.Play();
                }

                if (GUILayout.Button("Stop"))
                {
                    _actionInvoker.Stop();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void OnNewActionAddCallBack()
        {
            if (_optionsArray.Length == 0) return;

            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < _optionsArray.Length; i++)
            {
                menu.AddItem(new GUIContent(_optionsArray[i]), false, OnActionSelected, _actionTypesList[i]);
            }
            menu.ShowAsContext();
        }

        void OnActionSelected(object actionType)
        {
            GameObject gameObject = new GameObject();
            //gameObject.hideFlags = HideFlags.HideInHierarchy;
            gameObject.name = actionType.ToString().Split('.')[^1];
            gameObject.transform.parent = _actionInvoker.transform;
            AnimationAction action = gameObject.AddComponent((System.Type)actionType) as AnimationAction;
            Undo.RegisterCreatedObjectUndo(gameObject, $"Added {((System.Type)actionType).Name} Component");

            int arraySize = _actionArrayProperty.arraySize;
            _actionArrayProperty.InsertArrayElementAtIndex(arraySize);
            SerializedProperty newElement = _actionArrayProperty.GetArrayElementAtIndex(arraySize);
            newElement.objectReferenceValue = action;
            Undo.SetCurrentGroupName(_UndoAdd);

            serializedObject.ApplyModifiedProperties();
        }

        private void AddAction(System.Type type)
        {
            AnimationAction instance = (AnimationAction)System.Activator.CreateInstance(type);
            _actionInvoker.ActionList.Add(instance);
        }

        List<System.Type> GetActionSubtypes()
        {
            return System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => typeof(AnimationAction).IsAssignableFrom(type) && !type.IsAbstract).ToList();
        }

    }
}

#endif
