using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.GameApplicationConfig
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NotificationConfig", menuName = "BebiLibs/ApplicationConfigs/NotificationConfig", order = 0)]
    public class NotificationConfig : ScriptableObject
    {
        [SerializeField] private string _notificationTitle;
        [SerializeField] private string[] _notificationMessagesArray;
        [SerializeField] private string _notificationData;

        public string NotificationTitle => _notificationTitle;
        public string[] NotificationMessagesArray => _notificationMessagesArray;
        public string NotificationData => _notificationData;
    }
}

