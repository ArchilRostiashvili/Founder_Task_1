using BebiLibs.EditorExtensions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasCreatorContextMenu : MonoBehaviour
{
    [MenuItem("Assets/Sprite Atlas Tool/Create Sprite Atlas", false)]
    static void CreateSpriteAtlasFromSprites()
    {
        var assetPaths = ContextMenuHelper.GetAssetPathFromSelection();
        CreateSpriteAtlasFromAssets(assetPaths, false);
    }

    [MenuItem("Assets/Sprite Atlas Tool/Create Sprite Atlas", true)]
    static bool CreateSpriteAtlasFromSpritesValidator()
    {
        return Selection.assetGUIDs.Length > 0 & ContextMenuHelper.IfSelectionIsValidType<Sprite>();
    }


    [MenuItem("Assets/Sprite Atlas Tool/Create Sprite Atlas In New Folder", false)]
    static void CreateSpriteAtlasFromSpritesInNewFolder()
    {
        var assetPaths = ContextMenuHelper.GetAssetPathFromSelection();
        CreateSpriteAtlasFromAssets(assetPaths, true);
    }

    [MenuItem("Assets/Sprite Atlas Tool/Create Sprite Atlas In New Folder", true)]
    static bool CreateSpriteAtlasFromSpritesInNewFolderValidator()
    {
        return Selection.assetGUIDs.Length > 0 && ContextMenuHelper.IfSelectionIsValidType<Sprite>();
    }


    [MenuItem("Assets/Sprite Atlas Tool/Create Sprite Atlas From Folder", false)]
    static void CreateSpriteAtlasFromFolder()
    {
        var assetPaths = ContextMenuHelper.GetAssetPathFromSelection();
        CreateSpriteAtlasFromAssets(assetPaths, false);
    }

    [MenuItem("Assets/Sprite Atlas Tool/Create Sprite Atlas From Folder", true)]
    static bool CreateSpriteAtlasFromFolderInNewFolderValidator()
    {
        return Selection.assetGUIDs.Length > 0 && ContextMenuHelper.IsSelectionOnlyDirectories();
    }


    private static void CreateSpriteAtlasFromAssets(List<string> assetPathList, bool generateFolderFromAssets)
    {
        List<Object> objects = new List<Object>();

        string firstPath = assetPathList[0];
        string folderPath = Path.GetDirectoryName(firstPath);
        string temporalsName = Path.GetFileNameWithoutExtension(firstPath);

        string atlasName = EditorInputDialog.Show("Atlas Name Dialog", "Enter atlas name:", $"{temporalsName}_atlas", "Create", "Cancel");

        if (string.IsNullOrEmpty(atlasName))
        {
            Debug.Log("Atlas Name is empty");
            return;
        }

        if (generateFolderFromAssets)
        {
            var folderObj = CombineObjectsInFolder(assetPathList, folderPath, atlasName);
            objects.Add(folderObj);
        }
        else
        {
            for (int i = 0; i < assetPathList.Count; i++)
            {
                Object unityObjects = AssetDatabase.LoadAssetAtPath<Object>(assetPathList[i]);
                objects.Add(unityObjects);
            }
        }

        CreateAtlasFromObjects(objects, folderPath, atlasName);

    }

    private static Object CombineObjectsInFolder(List<string> assetPathList, string folderPath, string atlasName)
    {
        string newFolderName = atlasName + "_folder";
        string newFolderPath = Path.Combine(folderPath, newFolderName);
        if (!Directory.Exists(newFolderPath))
        {
            AssetDatabase.CreateFolder(folderPath, newFolderName);
        }

        AssetDatabase.Refresh();

        foreach (string assetPath in assetPathList)
        {
            string assetName = Path.GetFileName(assetPath);
            string newAssetPath = Path.Combine(newFolderPath, assetName);
            AssetDatabase.MoveAsset(assetPath, newAssetPath);
        }
        return AssetDatabase.LoadAssetAtPath<Object>(newFolderPath);
    }

    public static void CreateAtlasFromObjects(List<Object> objects, string folderPath, string atlasName)
    {
        SpriteAtlas atlas = CreateNewAtlas();
        SpriteAtlasExtensions.Add(atlas, objects.ToArray());
        string atlasPath = Path.Combine(folderPath, $"{atlasName}.spriteatlas");
        AssetDatabase.CreateAsset(atlas, atlasPath);
        EditorUtility.SetDirty(atlas);
    }

    private static SpriteAtlas CreateNewAtlas()
    {
        SpriteAtlas atlas = new SpriteAtlas();
        var settings = atlas.GetPackingSettings();
        settings.enableRotation = false;
        settings.enableTightPacking = false;
        settings.enableAlphaDilation = false;
        settings.padding = 8;
        atlas.SetPackingSettings(settings);
        return atlas;
    }
}
