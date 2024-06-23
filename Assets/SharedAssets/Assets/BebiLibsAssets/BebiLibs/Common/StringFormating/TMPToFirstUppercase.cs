using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BebiLibs
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TMP_Text))]
    public class TMPToFirstUppercase : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textToModify;

        [Header("As StandAlone")]
        [SerializeField] private bool _updateOnEnable = false;
        [TextArea(6, 100)]
        [HideField("updateOnEnable", true)]
        [SerializeField] private string _textValue;


        private void Awake()
        {
            _textToModify = gameObject.GetComponent<TMP_Text>();
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
            _textToModify.text = ToUpperCaseFirst(text);
        }

        public string ToUpperCaseFirst(string text)
        {
            Debug.Log("ToUpperCaseFirst " + text);
            return StringUtils.FirstCharToUpper(text);
        }
    }
}
