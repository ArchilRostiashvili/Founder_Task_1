using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    public class DeepLinkReceiver : MonoBehaviour
    {
        public delegate void DeepLinkReceivedDelegate(List<DeepLinkData> allDeepLinkUrls, DeepLinkData recededDeepLinkUrl);

        private static List<DeepLinkData> _DeepLinkDataList = new List<DeepLinkData>();
        private static event DeepLinkReceivedDelegate DeepLinkReceiverEvent;

        [Header("Editor: ")]
        [SerializeField] internal bool _logInEditor;
        [SerializeField] internal string _testUrl;

        private void Awake()
        {
            Application.deepLinkActivated += OnDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                OnDeepLinkActivated(Application.absoluteURL);
            }
        }

        public void OnDeepLinkActivated(string url)
        {
            Dictionary<string, string> gameURl = GetLinkParamsFromURL(url);
            DeepLinkData deepLinkData = new DeepLinkData(url, gameURl);
            UpdateDeepLinkData(deepLinkData);
            LogReceivedDeepLinkData();
            DeepLinkReceiverEvent?.Invoke(_DeepLinkDataList, deepLinkData);
        }


        private void UpdateDeepLinkData(DeepLinkData deepLinkData)
        {
            _DeepLinkDataList.Add(deepLinkData);
        }

        private void LogReceivedDeepLinkData()
        {
#if UNITY_EDITOR
            if (_logInEditor)
            {
                foreach (var links in _DeepLinkDataList)
                {
                    Debug.Log($"URL: {links.Url}");
                    foreach (var item in links.ParameterDictionary)
                    {
                        Debug.Log($"Parameter ID: {item.Key}, Data: {item.Value}");
                    }
                }
            }
#endif
        }

        public static void AddReceiveListener(DeepLinkReceivedDelegate onDeepLinkInitialized, bool immediate)
        {
            DeepLinkReceiverEvent += onDeepLinkInitialized;

            if (immediate && _DeepLinkDataList.Count > 0)
            {
                onDeepLinkInitialized?.Invoke(_DeepLinkDataList, _DeepLinkDataList[^1]);
            }
        }

        public static void RemoveReceiveListener(DeepLinkReceivedDelegate onDeepLinkInitialized)
        {
            onDeepLinkInitialized?.Invoke(_DeepLinkDataList, _DeepLinkDataList[^1]);
        }

        public static Dictionary<string, string> GetLinkParamsFromURL(string uri)
        {
            Dictionary<string, string> parsedURLData = new Dictionary<string, string>();
            string[] urlData = uri.Split("?");


            if (urlData.Length >= 2)
            {
                string[] subURLSpan = urlData[1..];
                string concatenatedData = string.Concat(subURLSpan);

                NameValueCollection data = HttpUtility.ParseQueryString(concatenatedData);
                for (int i = 0; i < data.Count; i++)
                {
                    string key = data.GetKey(i);
                    string value = data.Get(i);
                    parsedURLData.Add(key, value);
                }
            }
            return parsedURLData;
        }

    }

    [SerializeField]
    public class DeepLinkData
    {
        public System.DateTime ReceiveTime;
        public string Url;
        public Dictionary<string, string> ParameterDictionary = new Dictionary<string, string>();
        public bool IsUsed { get; private set; } = false;

        public void Use()
        {
            IsUsed = true;
        }

        public DeepLinkData(string url, Dictionary<string, string> parameterDictionary)
        {
            ReceiveTime = System.DateTime.Now;
            Url = url;
            ParameterDictionary = parameterDictionary;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(DeepLinkReceiver))]
    public class ManagerDeepLinkEditor : Editor
    {
        private DeepLinkReceiver _managerDeepLink;

        private void OnEnable()
        {
            _managerDeepLink = (DeepLinkReceiver)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (GUILayout.Button("Test Url"))
            {
                _managerDeepLink.OnDeepLinkActivated(_managerDeepLink._testUrl);
            }
        }
    }
#endif
}
