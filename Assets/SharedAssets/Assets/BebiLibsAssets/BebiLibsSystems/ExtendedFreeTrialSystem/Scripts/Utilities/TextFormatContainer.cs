using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ExtendedFreeTrialSystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "TextFormatContainer", menuName = "BebiLibs/ExtendedFreeTrialSystem/TextFormatContainer", order = 0)]
    public class TextFormatContainer : ScriptableObject
    {
        [SerializeField] private List<FormatStringData> _formatSystems = new List<FormatStringData>();

        public void SetString(string key, string value)
        {
            FormatStringData formatSystem = _formatSystems.Find(x => x.key == key);
            if (formatSystem != null)
            {
                formatSystem.SetValue(value);
            }
            else
            {
                Debug.LogError($"Unable To find FormatString with key {key}");
            }
        }

        public void AddString(string key, string value)
        {
            if (!ContainsKey(key))
            {
                _formatSystems.Add(FormatStringData.CreateNew(key, value));
            }
            else
            {
                Debug.LogError($"container already contanis key {key}");
            }
        }

        public bool ContainsKey(string key)
        {
            return _formatSystems.Find(x => x.key == key) != null;
        }

        public string ReplaceString(string text)
        {
            for (int i = 0; i < _formatSystems.Count; i++)
            {
                text = text.Replace(_formatSystems[i].key, _formatSystems[i].value);
            }
            return text;
        }
    }
}
