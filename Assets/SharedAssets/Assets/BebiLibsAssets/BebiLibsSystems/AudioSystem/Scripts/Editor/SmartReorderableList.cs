using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using CustomEditorUtilities;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using UnityEditorInternal;

public class SmartReorderableList<T> where T : ScriptableObject
{
    private List<T> _remainingTypes;
    private string[] _remainingTypeString;
    private ReorderableList _reorderableList;

    private SerializedObject _serializedObject;
    private SerializedProperty _originalListProperty;
    private IList<T> _excludeList;
    private string _propertyNameSource;

    public System.Action<T, SerializedProperty> OnNewElementCreate;
    public System.Action<T, SerializedProperty> OnNewElementCreateEnds;
    public System.Action<int> OnRemoveStart;
    public System.Action OnRemoveEnds;

    public Editor _sourceElementEditor;

    Dictionary<string, SerializedObject> _serializedObjectDict = new Dictionary<string, SerializedObject>();
    private string _audioTrackReference;

    public SmartReorderableList(SerializedObject serializedObject, SerializedProperty originalListProperty, IList<T> assetsInUse, IList<T> excludeList, string propertyNameSource, string audioTrackReference)
    {
        _serializedObject = serializedObject;
        _originalListProperty = originalListProperty;
        _propertyNameSource = propertyNameSource;
        _excludeList = excludeList;
        _serializedObjectDict = new Dictionary<string, SerializedObject>();
        _audioTrackReference = audioTrackReference;

        UpdateList(assetsInUse);

        _reorderableList = new ReorderableList(serializedObject, originalListProperty, false, false, true, true)
        {
            drawElementCallback = DrawElementCallBack,
            elementHeightCallback = GetElementHeight,
            onAddDropdownCallback = OnItemAddWithDropDown,
            onRemoveCallback = OnItemRemove,
            onSelectCallback = OnItemSelect
        };
    }

    private void OnItemSelect(ReorderableList list)
    {
        var element = _originalListProperty.GetArrayElementAtIndex(list.index);
        string guid = element.FindPropertyRelative(_audioTrackReference).FindPropertyRelative("m_AssetGUID").stringValue;
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
        if (asset != null)
        {
            _sourceElementEditor = Editor.CreateEditor(asset);
            EditorGUIUtility.PingObject(asset);
        }
    }

    public void UpdateList(IList<T> assetsInUse)
    {
        _serializedObjectDict.Clear();
        var allTypedElement = UnityFileUtils.FindScriptableOBjectByType<T>().Where(x => !_excludeList.Contains(x)).ToList();
        _remainingTypes = allTypedElement.Where(x => !assetsInUse.Contains(x)).ToList();
        _remainingTypeString = _remainingTypes.Select(x => x.name).ToArray();
    }

    private void OnItemRemove(ReorderableList list)
    {
        OnRemoveStart?.Invoke(list.index);
        ReorderableList.defaultBehaviours.DoRemoveButton(list);
        _serializedObject.ApplyModifiedProperties();
        OnRemoveEnds?.Invoke();
        _serializedObjectDict.Clear();
    }

    private float GetElementHeight(int index)
    {
        if (index < 0 || index >= _reorderableList.count)
        {
            return 0;
        }

        SerializedProperty actionProperty = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
        SerializedProperty property = actionProperty.FindPropertyRelative(_audioTrackReference);
        return EditorGUI.GetPropertyHeight(property, true);
    }

    private void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (index < 0 || index >= _reorderableList.count)
        {
            return;
        }

        SerializedProperty actionProperty = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
        string fieldName = "Element " + index;

        SerializedProperty languageProperty = actionProperty.FindPropertyRelative(_propertyNameSource);
        if (languageProperty != null && languageProperty.objectReferenceValue != null)
        {
            fieldName = languageProperty.objectReferenceValue.name;
        }
        SerializedProperty property = actionProperty.FindPropertyRelative(_audioTrackReference);
        EditorGUI.PropertyField(rect, property, new GUIContent(fieldName), true);
    }

    private bool OnCanRemoveItem(ReorderableList list)
    {
        return list.count > 0;
    }


    private void OnItemAddWithDropDown(Rect buttonRect, ReorderableList list)
    {
        _serializedObjectDict.Clear();
        if (_remainingTypeString.Length == 0) return;

        GenericMenu menu = new GenericMenu();
        if (_remainingTypes.Count > 1)
        {
            menu.AddItem(new GUIContent("[ Add All ]"), false, OnNewElementAddAll, null);
        }

        for (int i = 0; i < _remainingTypeString.Length; i++)
        {
            menu.AddItem(new GUIContent(_remainingTypeString[i]), false, OnNewElementAdd, _remainingTypes[i]);
        }
        menu.ShowAsContext();
    }

    private void OnNewElementAddAll(object actionType)
    {
        foreach (var item in _remainingTypes)
        {
            OnNewElementAdd(item);
        }
    }

    void OnNewElementAdd(object actionType)
    {
        int arraySize = _originalListProperty.arraySize;
        _originalListProperty.InsertArrayElementAtIndex(arraySize);
        SerializedProperty newElement = _originalListProperty.GetArrayElementAtIndex(arraySize);

        OnNewElementCreate?.Invoke((T)actionType, newElement);

        Undo.SetCurrentGroupName("Add Element To Array");
        _serializedObject.ApplyModifiedProperties();

        OnNewElementCreateEnds?.Invoke((T)actionType, newElement);
    }

    public void DrawList()
    {
        _reorderableList.DoLayoutList();

        if (_sourceElementEditor != null)
        {
            _sourceElementEditor.DrawDefaultInspector();
        }
    }

}
#endif