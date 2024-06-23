using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "DataLocalPack", menuName = "Modules/BalloonAdventure/DataLocalPack", order = 0)]
[System.Serializable]
public class DataLocalPack : ScriptableObject
{
    public string packID;
    public List<DataLocalItem> arrayItems;
}


#if UNITY_EDITOR
[CustomEditor(typeof(DataLocalPack))]
public class DataLocalPackEditor : Editor
{
    private DataLocalPack _dataLocalPack;

    private void OnEnable()
    {
        _dataLocalPack = (DataLocalPack)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Load Data To Elements") && EditorUtility.DisplayDialog("Load Data", "Populate Items Inside DataLocalPack with assistive elements, may cause unwanted behaviour", "ok", "cancel"))
        {
            List<DataLocalItem> dataItems = _dataLocalPack.arrayItems;

            Sprite[] big_icons = this.GetObjects<Sprite>("Big Icons", Application.dataPath, out string lastPath);
            Sprite[] small_icons = this.GetObjects<Sprite>("small Icons", lastPath, out string lastPathIcon);
            AudioClip[] audioFiles = this.GetObjects<AudioClip>("Audio Clip", lastPathIcon, out string _);

            for (int i = 0; i < dataItems.Count; i++)
            {
                dataItems[i].sprite = (Sprite)big_icons[i];
                dataItems[i].sprite_small = (Sprite)small_icons[i];
                dataItems[i].sound = (AudioClip)audioFiles[i];
                EditorUtility.SetDirty(dataItems[i]);
            }

            EditorUtility.SetDirty(_dataLocalPack);
        }

    }

    public T[] GetObjects<T>(string title, string defaultPath, out string lastPath) where T : UnityEngine.Object
    {
        string path = EditorUtility.OpenFolderPanel(title, defaultPath, " ");
        if (string.IsNullOrEmpty(path)) throw new System.Exception("Path is Null or Empty, Fix IT!!!");
        lastPath = System.IO.Path.GetDirectoryName(path);

        string trimmedPath = path.Replace(Application.dataPath, "");
        string searchPath = "Assets" + trimmedPath;
        string[] guids = AssetDatabase.FindAssets("t:Object", new string[] { searchPath });
        T[] objects = new T[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            objects[i] = asset;
        }

        Debug.Log(title + " " + objects.Length);
        return objects;
    }

}
#endif