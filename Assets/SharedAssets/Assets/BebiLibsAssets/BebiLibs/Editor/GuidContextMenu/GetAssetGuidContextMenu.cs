using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GetAssetGuidContextMenu : MonoBehaviour
{
    [MenuItem("Assets/Copy GUID", false)]
    static void CreateAudioTrack()
    {
        if (Selection.assetGUIDs.Length == 1)
        {
            GUIUtility.systemCopyBuffer = Selection.assetGUIDs[0];
        }
    }

    [MenuItem("Assets/Copy GUID", true)]
    static bool CreateAudioTrackValidate()
    {
        return Selection.assetGUIDs.Length == 1;
    }
}
