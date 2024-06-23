using UnityEngine;
using UnityEditor;

namespace Survey
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SurveyResultParser))]
    public class SurveyResultParserEditor : Editor
    {
        private SurveyResultParser _surveyResultParser;

        private void OnEnable()
        {
            _surveyResultParser = (SurveyResultParser)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Parse"))
            {
                _surveyResultParser.Parse();
            }
        }
    }
#endif
}
