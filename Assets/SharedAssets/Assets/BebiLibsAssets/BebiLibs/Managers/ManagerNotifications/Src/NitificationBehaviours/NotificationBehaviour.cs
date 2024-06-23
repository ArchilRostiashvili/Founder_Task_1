using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.Notification.GameNotificationBehaviour
{
    public abstract class NotificationBehaviour : MonoBehaviour
    {

        public List<string> activationID;

        public abstract void Init();
        public abstract void OnNotificationOpen(string data);
        public virtual void OnRemoteNotificationOpen(string key, string value) => Debug.Log($"Remote Notification: key {key}, value {value}");
        public abstract void UpdateNotification();
        public abstract bool CanRegisterNotification(string activationID);
        public abstract bool CompareData(string id);

        public abstract void RegisterTestNotification(string notificationID);
        public abstract void ActivateTestNotification();
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(NotificationBehaviour), true)]
    public class NotificationBehaviourEditor : Editor
    {
        private NotificationBehaviour _notificationBehaviour;

        private void OnEnable()
        {
            _notificationBehaviour = (NotificationBehaviour)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Activate test notification"))
            {
                _notificationBehaviour.ActivateTestNotification();
            }
        }
    }
#endif
}

