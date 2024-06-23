using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets.Settings;

public class AssetAddressableMakerWindow : EditorWindow
{

    [MenuItem("BebiLibs/Helper/Asset Addressable Maker Window")]
    private static void ShowWindow()
    {
        var window = GetWindow<AssetAddressableMakerWindow>();
        window.titleContent = new GUIContent("AssetAddressableMakerWindow");
        window.Show();
    }

    private string _groupName = "Shared Group";

    private void OnGUI()
    {

        _groupName = EditorGUILayout.TextField("Group Name", _groupName);

        if (GUILayout.Button("Make Selected Objects As Addressable") && EditorUtility.DisplayDialog("Warning", "This Will Make Selected Objects As Addressable", "Ok", "Cancel"))
        {
            MakeSelectedObjectsAsAddressable();
        }
    }

    private void MakeSelectedObjectsAsAddressable()
    {
        var selectedObjects = Selection.objects;
        Debug.Log("Length of Selected Objects: " + selectedObjects.Length);
        SetAddressableGroupButch<UnityEngine.Object>(selectedObjects, _groupName);
    }

    public static void SetAddressableGroupButch<T>(IEnumerable<T> values, string groupName) where T : UnityEngine.Object
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings)
        {
            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (!group)
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));

            Debug.Log(groupName + " group created");


            var entriesAdded = GetSameTypeAddressableAssetEntries(values, group);

            foreach (var item in entriesAdded)
            {
                Debug.Log("item: " + item.Key);
                group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, item.Value, false, true);
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, item.Value, true, false);
            }
        }
    }

    public static Dictionary<System.Type, List<AddressableAssetEntry>> GetSameTypeAddressableAssetEntries<T>(IEnumerable<T> values, AddressableAssetGroup group) where T : UnityEngine.Object
    {
        Dictionary<System.Type, List<AddressableAssetEntry>> typeToAddressableAssetEntries = new Dictionary<System.Type, List<AddressableAssetEntry>>();
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        foreach (var item in values)
        {
            if (item != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(item);
                var guid = AssetDatabase.AssetPathToGUID(assetPath);

                var entry = settings.FindAssetEntry(guid);
                if (entry == null)
                {
                    entry = settings.CreateOrMoveEntry(guid, group, false, false);
                    var type = item.GetType();
                    if (!typeToAddressableAssetEntries.ContainsKey(type))
                        typeToAddressableAssetEntries.Add(type, new List<AddressableAssetEntry>());

                    typeToAddressableAssetEntries[type].Add(entry);
                }
                else
                {
                    Debug.LogError("Entry exists in settings: " + guid + " and item: " + item.name);
                }
            }
        }

        return typeToAddressableAssetEntries;
    }
}