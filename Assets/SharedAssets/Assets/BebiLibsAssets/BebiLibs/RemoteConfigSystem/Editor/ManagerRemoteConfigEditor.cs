using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace BebiLibs.RemoteConfigSystem
{
#if UNITY_EDITOR
    [CustomEditor(typeof(RemoteConfigManager))]
    public class ManagerRemoteConfigEditor : Editor
    {

        private RemoteConfigManager _managerRemoteConfig;
        public RemoteConfigSettings dataRemoteVariables;


        public static string assetFileName = "remoteValue";

        public static string[] Options;

        public static System.Type[] listOfRVType;

        public static int selectedTypeIndex = 0;

        public static string[] elementOption;
        public static int selectedElementIndex = 0;
        public static int lastSelectedIndex = 0;

        public Editor selectedEditirWindow;
        public RemoteVariable selectedRemoteVariable;


        private void OnEnable()
        {
            listOfRVType = (
                   from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
                   from assemblyType in domainAssembly.GetTypes()
                   where typeof(RemoteVariable).IsAssignableFrom(assemblyType)
                   select assemblyType).ToArray();

            Options = new string[listOfRVType.Length];
            for (int i = 0; i < listOfRVType.Length; i++)
            {
                Options[i] = listOfRVType[i].Name;
            }

            _managerRemoteConfig = (RemoteConfigManager)target;
            UpdateRequirements();
            LoadSelectedEditor();
        }

        public void UpdateRequirements()
        {
            if (DoesDataExists(out dataRemoteVariables))
            {
                RemoteConfigManager.SetDataRemoteVariable(dataRemoteVariables);
                List<RemoteVariable> remoteVariables = dataRemoteVariables.RemoteVariableList;
                elementOption = new string[remoteVariables.Count];
                for (int i = 0; i < remoteVariables.Count; i++)
                {
                    string remoteVariableValue = remoteVariables[i].ToString();
                    elementOption[i] = remoteVariables[i].name + ": " + remoteVariableValue.Substring(0, Mathf.Clamp(remoteVariableValue.Length, 0, 30));
                }
            }
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Remote Variable PopUp:");

            if (dataRemoteVariables == null)
            {
                if (GUILayout.Button($"Create {nameof(RemoteConfigSettings)} Asset"))
                {
                    RemoteConfigManager.SetDataRemoteVariable(CreateDataRemoteVariable());
                    EditorUtility.SetDirty(_managerRemoteConfig);
                    UpdateRequirements();
                }
                EditorGUILayout.HelpBox($"Asset {nameof(RemoteConfigSettings)} Does Not Exists", MessageType.Error);
                return;
            }

            DisplaySelectedRemoteVariableEditor();

            DisplayCreatePopUp();
        }

        public void LoadSelectedEditor()
        {
            if (dataRemoteVariables != null)
            {
                selectedRemoteVariable = dataRemoteVariables.RemoteVariableList[selectedElementIndex];
                if (selectedRemoteVariable != null)
                {
                    selectedEditirWindow = Editor.CreateEditor(selectedRemoteVariable);
                }
            }
        }

        public void DisplaySelectedRemoteVariableEditor()
        {
            selectedElementIndex = EditorGUILayout.Popup(selectedElementIndex, elementOption);
            if (lastSelectedIndex != selectedElementIndex)
            {
                LoadSelectedEditor();
                lastSelectedIndex = selectedElementIndex;
            }

            if (selectedEditirWindow != null)
            {
                selectedEditirWindow.DrawDefaultInspector();
            }
        }

        public void DisplayCreatePopUp()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Remote Variable Creator:");

            EditorGUILayout.BeginHorizontal();
            float currentWidth = EditorGUIUtility.currentViewWidth;
            float currentLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = currentLabelWidth * 0.4f;
            assetFileName = EditorGUILayout.TextField("key", assetFileName, GUILayout.Width(currentWidth * 0.4f));
            EditorGUIUtility.labelWidth = currentLabelWidth;
            selectedTypeIndex = EditorGUILayout.Popup(selectedTypeIndex, Options);

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create new variable") && dataRemoteVariables != null)
            {
                Debug.Log(assetFileName + " " + listOfRVType[selectedTypeIndex]);
                dataRemoteVariables.CreateNewVariable(assetFileName, listOfRVType[selectedTypeIndex]);
            }

            EditorGUILayout.EndHorizontal();
        }

        public bool DoesDataExists(out RemoteConfigSettings dataRemote)
        {
            dataRemote = AssetDatabase.LoadAssetAtPath<RemoteConfigSettings>(dataRemoteAssetPath);
            return dataRemote != null;
        }

        public string dataRemoteAssetPath => Path.Combine("Assets", "Resources", nameof(RemoteConfigSettings) + ".asset");

        public RemoteConfigSettings CreateDataRemoteVariable()
        {
            string dirPath = Path.Combine(Application.dataPath, "Resources");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            ScriptableObject dataRemote = ScriptableObject.CreateInstance<RemoteConfigSettings>();
            AssetDatabase.CreateAsset(dataRemote, dataRemoteAssetPath);
            return (RemoteConfigSettings)dataRemote;
        }
    }
#endif
}
