#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AssetReferenceTuple
{
    public AssetReference AssetReference;
    public AddressableAssetEntry AddressableAssetEntry;
    public bool IsInitialized = false;

    public AssetReferenceTuple(AssetReference assetReference, AddressableAssetEntry addressableAssetEntry, bool isInitialized)
    {
        AssetReference = assetReference;
        AddressableAssetEntry = addressableAssetEntry;
        IsInitialized = isInitialized;
    }
}

public class AssetReferenceTuple<T> where T : UnityEngine.Object
{
    public AssetReferenceT<T> AssetReference;
    public AddressableAssetEntry AddressableAssetEntry;
    public bool IsInitialized = false;

    public AssetReferenceTuple(AssetReferenceT<T> assetReference, AddressableAssetEntry addressableAssetEntry, bool isInitialized)
    {
        AssetReference = assetReference;
        AddressableAssetEntry = addressableAssetEntry;
        IsInitialized = isInitialized;
    }
}

public interface IAddressableGenerator
{
    public UnityEngine.Object GetObject();
    public string GetLabel();
    public string GetPreloadLabel();
    void HandleGeneratedAssetReference(AssetReference assetReference);
}

public static class AddressableExtensions
{
    public static void SetAddressableGroup(UnityEngine.Object obj, string groupName)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings)
        {
            var group = settings.FindGroup(groupName);
            if (!group)
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));

            var assetpath = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(assetpath);

            var e = settings.CreateOrMoveEntry(guid, group, false, false);
            var entriesAdded = new List<AddressableAssetEntry> { e };

            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
        }
    }

    public static AssetReferenceT<T> SetAddressableGroup<T>(UnityEngine.Object obj, string groupName, string label = "", bool refreshLabels = false) where T : UnityEngine.Object
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings)
        {
            var group = settings.FindGroup(groupName);
            if (!group)
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));

            var assetPath = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(assetPath);

            AddressableAssetEntry newAddressableAsset = settings.CreateOrMoveEntry(guid, group, false, true);

            if (refreshLabels)
            {
                var labelsList = newAddressableAsset.labels.Select(x => x).ToList();
                foreach (string item in labelsList)
                {
                    newAddressableAsset.SetLabel(item, false, true, true);
                }
            }

            newAddressableAsset.SetLabel(label, true, true, true);

            List<AddressableAssetEntry> entriesAdded = new List<AddressableAssetEntry> { newAddressableAsset };

            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
            AssetReferenceT<T> assetReference = new AssetReferenceT<T>(guid);
            return assetReference;
        }
        else
        {
            return null;
        }
    }

    public static List<AssetReferenceTuple> SetAddressableGroupButch<T>(IEnumerable<T> values, string groupName, int length = 0, List<string> labels = null, string preloadLabel = null) where T : UnityEngine.Object
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings)
        {
            var group = settings.FindGroup(groupName);
            if (!group)
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));

            var assetReferenceTuple = new List<AssetReferenceTuple>(0);
            var entriesAdded = new List<AddressableAssetEntry>(length);

            int index = 0;
            foreach (var item in values)
            {
                if (item != null)
                {
                    var assetPath = AssetDatabase.GetAssetPath(item);
                    var guid = AssetDatabase.AssetPathToGUID(assetPath);

                    settings.RemoveAssetEntry(guid);
                    var entry = settings.CreateOrMoveEntry(guid, group, false, false);

                    entry.SetLabel(labels == null || labels.Count == 0 ? groupName : labels[index], true, true, true);
                    if (!string.IsNullOrEmpty(preloadLabel))
                    {
                        entry.SetLabel(preloadLabel, true, true, true);
                    }

                    AssetReference assetReference = new AssetReference(guid);
                    assetReferenceTuple.Add(new AssetReferenceTuple(assetReference, entry, true));
                    entriesAdded.Add(entry);
                }
                else
                {
                    assetReferenceTuple.Add(new AssetReferenceTuple(null, null, false));
                }

                index++;
            }

            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
            return assetReferenceTuple;
        }
        return null;
    }


    public static List<AssetReferenceTuple<T>> SetAddressableGroupButchT<T>(IEnumerable<T> values, string groupName, int length = 0, List<string> labels = null, string preloadLabel = null) where T : UnityEngine.Object
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings)
        {
            var group = settings.FindGroup(groupName);
            if (!group)
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));

            var assetReferenceTuple = new List<AssetReferenceTuple<T>>(0);
            var entriesAdded = new List<AddressableAssetEntry>(length);

            int index = 0;
            foreach (var item in values)
            {
                if (item != null)
                {
                    var assetPath = AssetDatabase.GetAssetPath(item);
                    var guid = AssetDatabase.AssetPathToGUID(assetPath);

                    settings.RemoveAssetEntry(guid);
                    var entry = settings.CreateOrMoveEntry(guid, group, false, false);

                    entry.SetLabel(labels == null || labels.Count == 0 ? groupName : labels[index], true, true, true);
                    if (!string.IsNullOrEmpty(preloadLabel))
                    {
                        entry.SetLabel(preloadLabel, true, true, true);
                    }

                    AssetReferenceT<T> assetReference = new AssetReferenceT<T>(guid);
                    assetReferenceTuple.Add(new AssetReferenceTuple<T>(assetReference, entry, true));
                    entriesAdded.Add(entry);
                }
                else
                {
                    assetReferenceTuple.Add(new AssetReferenceTuple<T>(null, null, false));
                }

                index++;
            }

            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
            return assetReferenceTuple;
        }
        return null;
    }


    public static void CreateAddressableWithGenerator<T>(IEnumerable<T> values, string groupName) where T : IAddressableGenerator
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings)
        {
            var group = settings.FindGroup(groupName);
            if (!group)
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));

            var entriesAdded = new List<AddressableAssetEntry>(values.Count());


            foreach (var item in values)
            {
                if (item != null || item.GetObject() != null)
                {
                    var assetPath = AssetDatabase.GetAssetPath(item.GetObject());
                    var guid = AssetDatabase.AssetPathToGUID(assetPath);

                    settings.RemoveAssetEntry(guid);
                    var entry = settings.CreateOrMoveEntry(guid, group, false, false);

                    string label = item.GetLabel();
                    if (!string.IsNullOrEmpty(label))
                        entry.SetLabel(label, true, true, true);

                    string preloadLabel = item.GetPreloadLabel();
                    if (!string.IsNullOrEmpty(preloadLabel))
                    {
                        entry.SetLabel(preloadLabel, true, true, true);
                    }

                    AssetReference assetReference = new AssetReference(guid);
                    entriesAdded.Add(entry);
                    item.HandleGeneratedAssetReference(assetReference);
                }
            }
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
        }
    }


    public static List<AssetReference> GetAddressableList(string groupName)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings)
        {
            List<AddressableAssetEntry> addressableAssetEntries = new List<AddressableAssetEntry>();
            List<AssetReference> assetReferences = new List<AssetReference>();
            var group = settings.FindGroup(groupName);
            if (!group)
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));

            group.GatherAllAssets(addressableAssetEntries, false, false, false);

            for (int i = 0; i < addressableAssetEntries.Count; i++)
            {
                AssetReference asset = new AssetReference(addressableAssetEntries[i].guid);
                assetReferences.Add(asset);
            }

            return assetReferences;
        }

        return null;
    }


    public static List<AddressableAssetEntry> GetAddressableEntryList(string groupName)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings)
        {
            List<AddressableAssetEntry> addressableAssetEntries = new List<AddressableAssetEntry>();
            var group = settings.FindGroup(groupName);
            if (!group)
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));

            group.GatherAllAssets(addressableAssetEntries, false, false, false);
            return addressableAssetEntries;
        }

        return null;
    }


}
#endif