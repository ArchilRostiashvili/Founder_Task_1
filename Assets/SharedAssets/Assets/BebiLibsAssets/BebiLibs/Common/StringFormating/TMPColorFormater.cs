using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.CommonLogic.TextFormatter
{
    [System.Serializable]
    public class TMPColorFormatter
    {
        [SerializeField] private string _startToken;
        [SerializeField] private string _endToken;
        [SerializeField] private Color _textColor;

        public TMPColorFormatter(string startToken, string endToken, Color textColor)
        {
            _startToken = startToken;
            _endToken = endToken;
            _textColor = textColor;
        }

        public TMPColorFormatter(string startToken, string endToken, string hexColor)
        {
            _startToken = startToken;
            _endToken = endToken;
            if (!ColorUtility.TryParseHtmlString(hexColor, out Color color))
            {
                Debug.Log($"Unable To Parse Color Value from hex {hexColor}");
            };

            _textColor = color;
        }

        public string FormatString(string textToFormat)
        {
            string startMarkup = $"<color={GetColorString()}>";
            string endMarkup = $"</color>";

            textToFormat = textToFormat.Replace(_startToken, startMarkup);
            textToFormat = textToFormat.Replace(_endToken, endMarkup);
            return textToFormat;
        }

        public string GetColorString()
        {
            return "#" + ColorUtility.ToHtmlStringRGBA(_textColor);
        }

        public void SetColor(Color newColor)
        {
            _textColor = newColor;
        }
    }
}
