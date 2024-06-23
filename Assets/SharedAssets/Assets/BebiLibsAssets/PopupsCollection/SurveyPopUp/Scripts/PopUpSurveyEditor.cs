using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PopUpSurveyEditor : MonoBehaviour
{

}

#if UNITY_EDITOR
[CustomEditor(typeof(PopUpSurveyEditor))]
public class PopUpSurveyEditorHelper : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Clean PlayerPrefs"))
        {
            PlayerPrefs.DeleteKey("SurveyStartedCountValue");
            PlayerPrefs.DeleteKey("SurveyExitPageValue");
            PlayerPrefs.DeleteKey("SurveyCompletionValue");
            PlayerPrefs.DeleteKey("SurveyAnsweredValues");
        }
    }
}
#endif