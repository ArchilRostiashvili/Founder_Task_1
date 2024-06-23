using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    public class PopUp_Alert : PopUpBase
    {
        private static PopUp_Alert _instance;

        private Coroutine _c;
        public TextMeshProUGUI Text_Message;

        public static PopUp_Alert Instance
        {
            get
            {
                return _instance;
            }
        }

        override public void Init()
        {
            base.Init();
            _instance = this;
        }

        public static void Activate(string message)
        {
            _instance.Show(true);

            if (LocalizationManager.TryGetTranslation(message, out string translation))
            {
                _instance.SetMessage(translation);
            }
            else
            {
                _instance.SetMessage(message);
            }
        }

        override public void Show(bool anim)
        {
            base.Show(anim);

            if (_c != null)
            {
                this.StopCoroutine(_c);
                _c = null;
            }

            _c = this.StartCoroutine(this.DelayClose(6.0f));
        }

        public void SetMessage(string message)
        {
            this.Text_Message.text = message;
        }

        override public void Hide(bool anim)
        {
            if (_c != null)
            {
                this.StopCoroutine(_c);
                _c = null;
            }
            base.Hide(anim);
        }

        private IEnumerator DelayClose(float time)
        {
            yield return new WaitForSeconds(time);
            this.Hide(true);
        }

        override public void Trigger_ButtonClick_Close()
        {
            ManagerSounds.PlayEffect("fx_page17");
            this.Hide(false);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PopUp_Alert))]
    public class PopUp_AlertEditor : Editor
    {
        private PopUp_Alert _popUp_Alert;

        private void OnEnable()
        {
            _popUp_Alert = (PopUp_Alert)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Activate"))
            {
                PopUp_Alert.Activate("TEXT_NO_INTERNET_CONNECTION");
            }
        }
    }
#endif

}
