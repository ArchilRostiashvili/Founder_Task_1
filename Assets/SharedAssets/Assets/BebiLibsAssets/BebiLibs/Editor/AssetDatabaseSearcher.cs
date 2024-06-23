// #if UNITY_EDITOR

// namespace BebiLibs
// {
//     using UnityEditor;
//     using UnityEngine;

//     public class AssetDatabaseSearcher : EditorWindow
//     {
//         private static AudioReference audioReference;
//         private static System.Action<string> callBack_OnConform;
//         private static AssetDatabaseSearcher window;

//         public static void ShowWindow(AudioReference audio, System.Action<string> Callback_OnConform)
//         {
//             window = GetWindow<AssetDatabaseSearcher>("AssetDatabase Example");
//             window.Show();
//             audioReference = audio;
//             callBack_OnConform = Callback_OnConform;
//         }
//         [SerializeField]
//         AutocompleteSearchField autocompleteSearchField;

//         public static void Hide()
//         {
//             window.Close();
//         }

//         void OnEnable()
//         {
//             if (autocompleteSearchField == null) autocompleteSearchField = new AutocompleteSearchField();
//             autocompleteSearchField.onInputChanged = this.OnInputChanged;
//             autocompleteSearchField.onConfirm = callBack_OnConform;
//         }

//         void OnGUI()
//         {
//             GUILayout.Label("Search AssetDatabase", EditorStyles.boldLabel);
//             autocompleteSearchField.OnGUI();
//         }

//         void OnInputChanged(string searchString)
//         {
//             autocompleteSearchField.ClearResults();
//             if (!string.IsNullOrEmpty(searchString))
//             {
//                 foreach (var assetGuid in AssetDatabase.FindAssets("t:AudioClip " + searchString))
//                 {
//                     var result = AssetDatabase.GUIDToAssetPath(assetGuid);
//                     if (result != autocompleteSearchField.searchString)
//                     {
//                         autocompleteSearchField.AddResult(result);
//                     }
//                 }
//             }
//         }

//         // void OnConfirm(string result)
//         // {
//         //     var obj = AssetDatabase.LoadMainAssetAtPath(autocompleteSearchField.searchString);
//         //     Selection.activeObject = obj;
//         //     EditorGUIUtility.PingObject(obj);
//         // }
//     }
// }
// #endif