using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BebiLibs
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    using UnityEngine;

    [ExecuteInEditMode]
    public class VersionDisplay : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField] private TMP_Text _versionNumberDisplayText;
        [SerializeField] private string _buildNumber;

        public void OnAfterDeserialize()
        {

        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR && UNITY_ANDROID
            _buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
#elif UNITY_EDITOR && UNITY_IOS
            _buildNumber = PlayerSettings.iOS.buildNumber;
#endif      
        }

        private void OnEnable()
        {
            _versionNumberDisplayText.text = Application.version + "." + _buildNumber;
        }
    }
}
