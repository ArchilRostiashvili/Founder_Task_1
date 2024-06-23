using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public class AddressableAtlasCleaner : EditorWindow
{

    [MenuItem("BebiLibs/Helper/AddressableAtlasCleaner")]
    private static void ShowWindow()
    {
        var window = GetWindow<AddressableAtlasCleaner>();
        window.titleContent = new GUIContent("Addressable Atlas Cleaner");
        window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Clean Addressable Atlases"))
        {
            CleanTexturesWithAtlases();
        }
    }


    public void CleanTexturesWithAtlases()
    {
        var allSpriteAtlases = GetAllSpriteAtlases();
        var allSprites = GetAllSpritesInsideAddressable();
        Debug.Log("Found " + allSpriteAtlases.Count + " sprite atlases");
        foreach (var atlas in allSpriteAtlases)
        {
            Debug.Log("Process atlas named: " + atlas.name);
            RemoteAtlasSpritesFromAddressables(atlas, allSprites, 2);
        }
    }

    public List<Sprite> GetAllSpritesInsideAddressable()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        List<AddressableAssetEntry> assetEntrys = new List<AddressableAssetEntry>();
        settings.GetAllAssets(assetEntrys, true, null, null);
        List<Sprite> sprites = new List<Sprite>();
        foreach (var assetEntry in assetEntrys)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetEntry.guid);
            var asset = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (asset is not null)
            {
                sprites.Add(asset);
            }
        }
        return sprites;
    }

    // Remove Sprites From SpriteAtlas if Atlas is inside Addressable
    public void RemoteAtlasSpritesFromAddressables(SpriteAtlas atlas, List<Sprite> allSprites, int indent)
    {
        if (IsAtlasInsideAddressable(atlas, indent))
        {
            List<Sprite> sprites = GetSpritesFromAtlas(atlas, allSprites, 2 * indent);
            foreach (var sprite in sprites)
            {
                RemoveSpriteFromAddressable(sprite, 4 * indent);
            }
        }
    }

    public List<Sprite> GetSpritesFromAtlas(SpriteAtlas atlasAsset, List<Sprite> allSprites, int indent)
    {
        List<Sprite> sprites = new List<Sprite>();
        foreach (var item in allSprites)
        {
            if (atlasAsset.CanBindTo(item))
            {
                sprites.Add(item);
            }
        }
        return sprites;
    }

    public bool IsAtlasInsideAddressable(SpriteAtlas atlas, int indent)
    {
        string assetPath = AssetDatabase.GetAssetPath(atlas);
        Debug.Log($"{GetIndentString(indent)} Check if {assetPath} is inside addressable");
        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        bool isInside = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid) != null;
        return isInside;
    }


    public void RemoveSpriteFromAddressable(Sprite sprite, int indent)
    {
        string assetPath = AssetDatabase.GetAssetPath(sprite);
        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        bool isSuccessfull = AddressableAssetSettingsDefaultObject.Settings.RemoveAssetEntry(guid);
        if (isSuccessfull)
        {
            Debug.Log($"{GetIndentString(indent)} Successfully Removed {sprite.name} from addressable");
        }
        else
        {
            Debug.Log($"{GetIndentString(indent)} Failed to remove {sprite.name} from addressable");
        }
    }

    public List<SpriteAtlas> GetAllSpriteAtlases()
    {
        List<SpriteAtlas> spriteAtlases = new List<SpriteAtlas>();
        string[] guids = AssetDatabase.FindAssets("t:SpriteAtlas");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            spriteAtlases.Add(spriteAtlas);
        }
        return spriteAtlases;
    }

    private string GetIndentString(int spaceCount)
    {
        return new string(' ', spaceCount);
    }
}