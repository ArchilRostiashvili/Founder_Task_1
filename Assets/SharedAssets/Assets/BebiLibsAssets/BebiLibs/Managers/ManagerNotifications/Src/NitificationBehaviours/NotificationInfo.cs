using UnityEngine;

namespace BebiLibs.Notification.GameNotificationBehaviour
{
    [System.Serializable]
    public class NotificationInfo
    {
        [SerializeField] private string _notificationID;
        [SerializeField] private string _notificationContent;

        public string notificationID => _notificationID;
        public string notificationContent => _notificationContent;
    }
}

