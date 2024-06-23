using DG.Tweening;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using BebiLibs.Analytics;
using I2.Loc;
using BebiLibs.GameApplicationConfig;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    public class ManagerApp : GenericSingletonClass<ManagerApp>
    {
        public static long GoOutTimeSnap = 0;
        public static string GoOutFromWhere = null;

        public static ApplicationConfig Config => Instance.GetConfig();

        [SerializeField] private ApplicationConfig _managerAppConfig;

        protected override void OnInstanceAwake()
        {
            _dontDestroyOnLoad = false;
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(100, 20);
        }

        internal ApplicationConfig GetConfig()
        {
            if (_managerAppConfig == null)
            {
                _managerAppConfig = ApplicationConfigProvider.DefaultInstance().CurrentConfig;
            }
            return _managerAppConfig;
        }

        public void OnApplicationPause(bool paused)
        {
            if (paused == false)
            {
                if (GoOutTimeSnap > 0)
                {
                    long diff = TimeUtils.UnixTimeInSeconds() - GoOutTimeSnap;
                    if (diff >= 8)
                    {
                        SharedAnalyticsManager.RateUnlockEvent("come_back", TimeUtils.UnixTimeInSeconds() - GoOutTimeSnap);

                    }
                }
                GoOutTimeSnap = 0;
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ManagerApp))]
    public class ManagerAppEditor : Editor
    {
        private ManagerApp _managerApp;

        public Editor dataManagerAppEditor;
        private void OnEnable()
        {
            _managerApp = (ManagerApp)target;
            dataManagerAppEditor = Editor.CreateEditor(_managerApp.GetConfig());
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Manager App Config Data: ");
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            if (dataManagerAppEditor != null)
            {
                dataManagerAppEditor.DrawDefaultInspector();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }
#endif
}