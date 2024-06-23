using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.RegistrationSystem
{
    public class RegistrationDeepLinkListener : MonoBehaviour
    {
        [SerializeField] internal string _loginDoneTestURL = "bebi.family.kids.learning.games://loginDone?token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJiZGJlNTNmNi0zNGViLTRjMWItYmZiNy1jMjVhYjExMTM3MTQiLCJzaWQiOiI5NDdlY2RkYy0zNWRiLTQwODEtYWMwZS1jODgxNzgxZjE1YmMiLCJkZXYiOiJiNDM0NzhhMC05ZjBhLTQ4NzctYTZlNC1jM2M3NWViZGY5ZGQiLCJuYW1lIjoiVGVkbyBHYWtoYXJpYSIsInN1YiI6ImJlMzVmYjI1LTE1MDEtNDhkMC1hMGFlLWRiYWU3ZjczM2YxNiIsImVtYWlsIjoidC5nYWtoYXJpYUBmbmRyLmdlIiwibmJmIjoxNjQ1NTUyMjEyLCJleHAiOjE2NDY3NjE4MTIsImlhdCI6MTY0NTU1MjIxMiwiaXNzIjoiYmViaS1mYW1pbHkuZm5kci5kZXYiLCJhdWQiOiJmNDYzYTJlOC0xOGU4LTQ0MzYtYThhNS01YmFiNWQ4MDcxN2QifQ.UH5Gia86_Q7iFtC4PoOo3W9LKhAbrI2jFZIBWS57RvE";
        [SerializeField] internal string _loginFailTestURL = "bebi.family.kids.learning.games://loginFail?error=unable to sign in facebook";

        public event System.Action<string> Callback_OnDeepLinkLoginDone;
        public event System.Action<string> Callback_OnDeepLinkLoginFail;

        private void Awake()
        {
            Application.deepLinkActivated += this.OnDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                this.OnDeepLinkActivated(Application.absoluteURL);
            }
        }

        internal void OnDeepLinkActivated(string absoluteDeeplinkURL)
        {
            if (absoluteDeeplinkURL.Contains("loginDone"))
            {
                this.ParseLoginDone(absoluteDeeplinkURL);
            }
            else if (absoluteDeeplinkURL.Contains("loginFail"))
            {
                this.ParseLoginFail(absoluteDeeplinkURL);
            }
        }

        private void ParseLoginDone(string url)
        {
            string[] urlParts = url.Split("token=");
            if (urlParts.Length == 2)
            {
                string token = urlParts[1];
                Callback_OnDeepLinkLoginDone?.Invoke(token);
            }
            else
            {
                Debug.LogWarning($"Unable To Parse Login Done Url {url}");
            }
        }

        private void ParseLoginFail(string url)
        {
            string[] urlParts = url.Split("error=");
            if (urlParts.Length == 2)
            {
                string error = urlParts[1];
                Callback_OnDeepLinkLoginFail?.Invoke(error);
            }
            else
            {
                Debug.LogWarning($"Unable To Parse Login Fail Url {url}");
            }
        }

        private void OnDestroy()
        {
            Application.deepLinkActivated -= this.OnDeepLinkActivated;
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RegistrationDeepLinkListener))]
    public class RegistrationDeepLinkListenerEditor : Editor
    {
        private RegistrationDeepLinkListener _managerDeepLink;

        private void OnEnable()
        {
            _managerDeepLink = (RegistrationDeepLinkListener)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Editor: ");
            if (GUILayout.Button("Test Login Done Url"))
            {
                _managerDeepLink.OnDeepLinkActivated(_managerDeepLink._loginDoneTestURL);
            }

            if (GUILayout.Button("Test Login Fail Url"))
            {
                _managerDeepLink.OnDeepLinkActivated(_managerDeepLink._loginFailTestURL);
            }
        }
    }
#endif
}

