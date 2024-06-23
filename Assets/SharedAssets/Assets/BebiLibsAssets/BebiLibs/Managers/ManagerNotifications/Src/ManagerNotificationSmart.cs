using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.Notification
{
    public class ManagerNotificationSmart : MonoBehaviour
    {
        [SerializeField] private string _smallIcon = "notification_small";
        [SerializeField] private string _largeIcon = "notification_large";
        private string _sound = "notification_sound";

        public static event Action<NotificationEventData> CallBack_OnNotificationReceive;
        public static ManagerNotificationSmart Instance { get; private set; }
        private static INotificationPublisher _publisher;
        public static bool IsInitialized { get; private set; } = false;

        private void Awake()
        {
            Instance = this;
        }

        public static void InitNotifications()
        {
            if (Instance == null)
            {
                Debug.LogError("Make Sure that instance of Manager Notification New Exists");
                return;
            }
            else
            {
                Instance.Init();
            }
        }

        public void Init()
        {
            try
            {
#if UNITY_EDITOR
                _publisher = new TestNotificationPublisher();
#elif UNITY_IOS
                _publisher = new IosNotificationPublisher();
#elif UNITY_ANDROID
                _publisher = new AndroidNotificationPublisher();   
#else
                _publisher = new TestNotificationPublisher();
#endif

                _publisher.CallBackValid = (bool valid) =>
                {
#if UNITY_IOS
                    //Checking if User is agree to get notifications
                    if (PlayerPrefs.GetInt("FirstValidation", 0) == 0)
                    {
                        PlayerPrefs.SetInt("FirstValidation", 1);
                    }
#endif
                    NotificationEventData eventData = new NotificationEventData(NotificationSource.Initialization, valid, string.Empty);
                    CallBack_OnNotificationReceive?.Invoke(eventData);

                    IsInitialized = true;
                    if (!valid)
                    {
                        Debug.LogWarning("Notification Is System Offline");
                    }
                };

                _publisher.Init(this);

                string notificationData = _publisher.GetNotificationData();
                if (notificationData != null)
                {
                    NotificationEventData eventData = new NotificationEventData(NotificationSource.UserInteraction, true, notificationData);
                    CallBack_OnNotificationReceive?.Invoke(eventData);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Notification Error" + e);
            }
        }




        public static void RegisterNotification(DateTime notificationTime, TimeSpan repeatInterval, string title = null, string content = null, string data = null)
        {
            if (Instance == null)
            {
                Debug.LogError("Make Sure that instance of Manager Notification New Exists");
                return;
            }

            // if (Debug.isDebugBuild)
            // {   
            //     Debug.Log($"Notification, Time: \"{notificationTime}\", Content: \"{ content}\", Data: \"{ data}\"");
            // }

            Instance.PublishLocalNotification(notificationTime, repeatInterval, title, content, data);
        }

        public static void ClearAllNotifications()
        {
            if (Instance == null)
            {
                Debug.LogError("Make Sure that instance of Manager Notification New Exists");
                return;
            }

            Instance.CancelAllNotifications();
        }

        public void PublishLocalNotification(DateTime notificationTime, TimeSpan repeatInterval, string title = null, string content = null, string data = null)
        {
            GameNotification gameNotification = new GameNotification()
            {
                notificationTime = notificationTime,
                title = title,
                content = content,
                data = data,
                largeIcon = _largeIcon,
                smallIcon = _smallIcon,
                sound = _sound,
                badge = 0,
                repeatInterval = repeatInterval,
                canRepeat = repeatInterval != TimeSpan.Zero
            };

            if (_publisher != null)
            {
                _publisher.PublishLocalNotification(gameNotification);
            }
            else
            {
                Debug.LogError("Unabel To Cancel Notifications, Make Sure that Notification Manager is initialized properly");
            }
        }

        public void CancelAllNotifications()
        {
            if (_publisher != null)
            {
                _publisher.RevokeAllLocalNotifications();
            }
            else
            {
                Debug.LogError("Unabel To Cancel Notifications, Make Sure that Notification Manager is initialized properly");
            }
        }


    }
}
