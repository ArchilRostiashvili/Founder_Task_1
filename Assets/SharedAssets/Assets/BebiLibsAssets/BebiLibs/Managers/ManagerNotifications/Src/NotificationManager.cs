using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs.Notification.GameNotificationBehaviour;
using BebiLibs.Notification;
using System;
using BebiLibs.PurchaseSystem;

#if ACTIVATE_MESSAGING
using Firebase.Messaging;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

using BebiLibs.Analytics;
using BebiLibs;
using BebiLibs.RemoteConfigSystem;
using BebiLibs.PurchaseSystem.Core;
using BebiLibs.Localization;
using BebiLibs.Localization.Core;

namespace BebiLibs
{
    public class NotificationManager : MonoBehaviour
    {
        private static NotificationManager _instance;
        public static PersistentTime FistGameInstallTime = new PersistentTime("GameFirstInstall_Notification", System.DateTime.Now);
        public static PersistentTime LastGameOpenTime = new PersistentTime("LastGameOpenTime_Notification", System.DateTime.Now);
        public static PersistentBoolean IsFirstOpen = new PersistentBoolean("GameFirstOpen_Notification", true);

        private static PersistentInteger _lastRegisteredNotIndex = new PersistentInteger("RegisteredLocalNotIndex", 0);
        private static PersistentTime _lastRegisteredNotTime = new PersistentTime("RegisteredLocalNotTime", DateTime.Today);

        [Header("Local Notification List:")]
        [SerializeField] private List<NotificationBehaviour> arrayLocalNotification = new List<NotificationBehaviour>();

        [Header("Remote Notification List:")]
        [SerializeField] private List<NotificationBehaviour> arrayRemoteNotification = new List<NotificationBehaviour>();

        [SerializeField] private PurchaseManagerBase _purchaseManager;


        // [Tooltip("Making value large can cause notification spamming, ideal value is 1")]
        // [Header("Notification Registration Settings:")]
        // [Range(1, 5)]
        // [SerializeField] private int _maxActiveNotificationCount = 2;
        // [Tooltip("Notification Registartion Deilay in Days")]
        // [Range(1, 5)]
        // [SerializeField] private int _maxNotificationDelay = 1;

        //Disable Notification Registration Until necessary rescuers ara available
        private bool _canUpdateSendNotifications = false;
        //ID Retrived from RemoteConfig
        //private int _remoteActivationID = 0;

        //private bool _isPushNotificationReceived = false;

#if ACTIVATE_MESSAGING
        private NotificationMessage _pushMessage;
#endif

        private void Awake()
        {
            _instance = this;
            ManagerNotificationSmart.CallBack_OnNotificationReceive += OnNotificationReceive;
            FirebaseDependencyResolver.AddInitializationListener(OnFirebaseResolve, true);
        }

        private void Start()
        {
            if (IsFirstOpen.isInitialized)
            {
                FistGameInstallTime.SetValue(System.DateTime.Now);
                IsFirstOpen.SetValue(false);
            }
            LastGameOpenTime.SetValue(System.DateTime.Now);

            StartCoroutine(OnResourcesInitializationFinished());

            IEnumerator OnResourcesInitializationFinished()
            {
                yield return new WaitForSeconds(2);

                float RemoteConfigWaitTime = RemoteConfigManager.TimeOutInSeconds + 1f;
                yield return new WaitForDone(RemoteConfigWaitTime, () => FirebaseDependencyResolver.IsInitializationFinished && RemoteConfigManager.IsLoadFinished);
                yield return new WaitForDone(15f, () => _purchaseManager.IsInitialized);
                _canUpdateSendNotifications = true;

                //RemoteConfig.TryGetInt("notification_churn", out _remoteActivationID, true);

                InitializeNotifications();
                UpdateNotifications();
            }
        }


        public void OnFirebaseResolve(bool result)
        {
#if ACTIVATE_MESSAGING
            if (result == true)
            {
                try
                {
                    FirebaseMessaging.MessageReceived += OnMessageReceived;
                    FirebaseMessaging.TokenReceived += OnTokenReceived;
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Firebase Notification Registration Call Error: " + e);
                }
            }
#endif
        }

        private void OnDestroy()
        {
#if ACTIVATE_MESSAGING
            try
            {
                FirebaseMessaging.MessageReceived -= OnMessageReceived;
                FirebaseMessaging.TokenReceived -= OnTokenReceived;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Firebase Notification Unregister Call Error: " + e);
            }


            StopAllCoroutines();
#endif
        }

#if ACTIVATE_MESSAGING
        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            try
            {
                // Debug.LogError(e.Message.MessageId);
                // Debug.LogError(e.Message.From);
                // foreach (var item in e.Message.Data)
                // {
                //     Debug.LogError(item.Key + " " + item.Value);
                // }
                _pushMessage = new NotificationMessage(e.Message.MessageId, e.Message.Data, e.Message.From)
                {
                    error = e.Message.Error,
                    errorDescription = e.Message.ErrorDescription
                };
                _isPushNotificationReceived = true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Notification Receive Failed " + ex);
            }
        }


        public void ReceiveNotificationsOnMainThread()
        {
            if (_pushMessage != null)
            {
                //UnityEngine.Debug.Log("From: " + _pushMessage.From);
                //UnityEngine.Debug.Log("Message ID: " + _pushMessage.MessageId);
                //Debug.LogError("Message Error: " + _pushMessage.error);

                if (_pushMessage.Data != null)
                {
                    foreach (var item in _pushMessage.Data)
                    {
                        Debug.Log(item.Key + " " + item.Value);
                        OnRemoteNotificationReceive(item.Key, item.Value);
                    }
                }
            }
            else
            {
                Debug.LogError("Firebase Notification Error");
            }
        }
#endif


        public void OnRemoteNotificationReceive(string key, string value)
        {
            foreach (var item in arrayRemoteNotification)
            {
                AnalyticsManager.LogEvent("n_login", "type", key);
                item.OnRemoteNotificationOpen(key, value);
            }
        }

#if ACTIVATE_MESSAGING
        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs e)
        {
            Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, "FCM Token: {0}", e.Token);
        }
#endif

        public void OnNotificationReceive(NotificationEventData notificationEventData)
        {
            if (notificationEventData.source == NotificationSource.UserInteraction)
            {
                AnalyticsManager.LogEvent("n_login", "type", notificationEventData.notificationData);

                foreach (var item in arrayLocalNotification)
                {
                    if (item.CompareData(notificationEventData.notificationData))
                    {
                        item.OnNotificationOpen(notificationEventData.notificationData);
                    }
                }
            }
        }

        private void OnEnable()
        {
            //_purchaseManager.PurchaseStateUpdatedEvent += OnUpdateNotification;
            _purchaseManager.PurchaseInitializeEvent += OnPurchaseInitialize;
            LocalizationManager.OnLanguageChangeEvent += OnLanguageChange;
        }

        private void OnDisable()
        {
            //_purchaseManager.PurchaseStateUpdatedEvent -= OnUpdateNotification;
            _purchaseManager.PurchaseInitializeEvent -= OnPurchaseInitialize;
            LocalizationManager.OnLanguageChangeEvent -= OnLanguageChange;
        }

        public void OnLanguageChange(LanguageIdentifier languageIdentifier)
        {
            UpdateNotifications();
        }

#if ACTIVATE_MESSAGING
        private void Update()
        {
            if (_isPushNotificationReceived)
            {
                ReceiveNotificationsOnMainThread();
                _isPushNotificationReceived = false;
            }
        }
#endif

        public void InitializeNotifications()
        {
            foreach (var item in arrayLocalNotification)
            {
                item.Init();
            }
        }

        public void OnUpdateNotification(ProductIdentifier productInfo, bool update)
        {
            UpdateNotifications();
        }

        public void OnPurchaseInitialize()
        {
            UpdateNotifications();
        }

        public void UpdateNotifications()
        {
            if (!_canUpdateSendNotifications) return;
            ManagerNotificationSmart.ClearAllNotifications();

            foreach (var item in arrayLocalNotification)
            {
                item.UpdateNotification();
            }

            if (Debug.isDebugBuild == false)
            {
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "update notifications");
            }

            // bool isThisCurrentDay = _lastRegisteredNotTime.GetValue().Date == DateTime.Today.Date;
            // int nextNotIndex = isThisCurrentDay ? _lastRegisteredNotIndex : (_lastRegisteredNotIndex + 1) % arrayLocalNotification.Count;

            // for (int i = nextNotIndex; i < arrayLocalNotification.Count + nextNotIndex; i++)
            // {
            //     int index = i % arrayLocalNotification.Count;
            //     if (arrayLocalNotification[index].CanRegisterNotification(_remoteActivationID.ToString()))
            //     {
            //         arrayLocalNotification[index].UpdateNotification();
            //         _lastRegisteredNotIndex.SetValue(index);
            //         _lastRegisteredNotTime.SetValue(DateTime.Today);
            //         break;
            //     }
            // }
        }

        // tests
        public static void ReceiveTestNotification(string data)
        {
            _instance.OnNotificationReceive(new NotificationEventData(NotificationSource.UserInteraction, true, data));
        }

        public static void ClearTestNotifications()
        {
            ManagerNotificationSmart.ClearAllNotifications();
        }

        public static void RegisterTestNotification(string data)
        {
            foreach (var item in _instance.arrayLocalNotification)
            {
                item.RegisterTestNotification(data);
            }
        }

        internal void RewindLastRegisteredNotificationTime()
        {
            _lastRegisteredNotTime.SetValue(DateTime.Today.Subtract(TimeSpan.FromDays(1)));
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(NotificationManager))]
    public class NotificationControllerEditor : Editor
    {
        private NotificationManager _notificationController;

        private void OnEnable()
        {
            _notificationController = (NotificationManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Update Notification"))
            {
                _notificationController.UpdateNotifications();
            }

            if (GUILayout.Button("Rewind Last Registered Notification Time"))
            {
                _notificationController.RewindLastRegisteredNotificationTime();
            }
        }
    }
#endif
}
