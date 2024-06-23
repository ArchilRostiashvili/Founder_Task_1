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
    public class TextContainerProcessor : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textComponent;
        [SerializeField] private RectTransform _containerRT;

        [SerializeField] private float _widthPading = 0;
        [SerializeField] private float _maxRectWidth = 500;
        [SerializeField] private float _minRectWidth = 100;
        [SerializeField] private float _textToRextSizeRatio = 38;

        private void OnValidate()
        {
            UpdateContainerProportion();
        }

        public string GetText()
        {
            return _textComponent.text;
        }

        public void SetText(string newText)
        {
            _textComponent.text = newText;
            UpdateContainerProportion();
        }

        public void UpdateContainerProportion()
        {
            int Length = _textComponent.text.Length;
            Vector3 newSizeDelta = _containerRT.sizeDelta;
            newSizeDelta.x = Mathf.Clamp(Length * _textToRextSizeRatio, _minRectWidth, _maxRectWidth) + _widthPading;
            _containerRT.sizeDelta = newSizeDelta;
        }
    }
}
