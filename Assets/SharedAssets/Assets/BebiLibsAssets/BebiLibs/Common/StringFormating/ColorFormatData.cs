using UnityEngine;

namespace BebiLibs
{
    [CreateAssetMenu(fileName = "FormatSystem", menuName = "BebiLibs/FormatSystem/ColorFormatData", order = 0)]
    public class ColorFormatData : StringFormatBase
    {
        [SerializeField] private string _key;

        [SerializeField] bool _isColorEnd;
        [HideField("_isColorEnd", false)]
        [SerializeField] private Color _color = Color.white;

        public override string key => _key;
        public override string value => _isColorEnd ? TMPFormatterUtility.GetColorEndMarkup() : TMPFormatterUtility.GetColorStartMarkup(_color);

        public static ColorFormatData CreateNew(string key, Color value)
        {
            ColorFormatData formatStingData = ScriptableObject.CreateInstance<ColorFormatData>();
            formatStingData._key = key;
            formatStingData._color = value;
            return formatStingData;
        }

        public void SetValue(Color value)
        {
            _color = value;
        }
    }
}

