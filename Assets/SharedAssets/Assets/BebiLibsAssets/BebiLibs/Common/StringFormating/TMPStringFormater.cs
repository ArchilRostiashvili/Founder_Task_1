using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TMP_Text))]
    public class TMPStringFormater : MonoBehaviour
    {
        public TMP_Text Text_TextToModify;
        public List<StringFormatBase> arrayFormatSystem = new List<StringFormatBase>();

        [Header("As StandAlone")]
        [UnityEngine.Serialization.FormerlySerializedAs("updateOnEnable")]
        [SerializeField] private bool _updateOnEnable = false;
        [TextArea(6, 100)]
        [UnityEngine.Serialization.FormerlySerializedAs("textValue")]
        [SerializeField] private string _textValue;

        public string GetStaticText => _textValue;
        public string GetText => Text_TextToModify.text;

        private void Awake()
        {
            Text_TextToModify = gameObject.GetComponent<TMP_Text>();
        }

        private void OnValidate()
        {
            OnEnable();
        }

        private void OnEnable()
        {
            if (_updateOnEnable)
            {
                UpdateText(_textValue);
            }
        }

        public void UpdateText(string text)
        {
            Text_TextToModify.text = ReplaceString(text);
        }

        public string ReplaceString(string text)
        {
            for (int i = 0; i < arrayFormatSystem.Count; i++)
            {
                text = text.Replace(arrayFormatSystem[i].key, arrayFormatSystem[i].value);
            }
            return text;
        }
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(TMPStringFormater))]
    public class TMPStringFormaterEditor : Editor
    {
        private TMPStringFormater _tMPStringFormater;

        private void OnEnable()
        {
            _tMPStringFormater = (TMPStringFormater)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Update Text"))
            {
                _tMPStringFormater.UpdateText(_tMPStringFormater.GetStaticText);
            }

            if (GUILayout.Button("Test"))
            {
                Debug.Log(_tMPStringFormater.ReplaceString("Try {0}Free{1} for {0}7{1} Days"));
            }
        }
    }
#endif
}
