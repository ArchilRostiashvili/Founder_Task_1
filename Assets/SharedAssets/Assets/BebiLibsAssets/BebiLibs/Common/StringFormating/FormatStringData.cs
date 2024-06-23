using UnityEngine;

namespace BebiLibs
{
    [CreateAssetMenu(fileName = "FormatSystem", menuName = "BebiLibs/FormatSystem/FormatStringData", order = 0)]
    public class FormatStringData : StringFormatBase
    {
        [SerializeField] private string _key;
        [SerializeField] private string _value;

        public override string key => _key;
        public override string value => _value;

        public static FormatStringData CreateNew(string key, string value)
        {
            FormatStringData formatStingData = ScriptableObject.CreateInstance<FormatStringData>();
            formatStingData._key = key;
            formatStingData._value = value;
            return formatStingData;
        }

        public void SetValue(string value)
        {
            _value = value;
        }
    }
}

