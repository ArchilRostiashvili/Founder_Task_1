#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Notifications.Android;
using UnityEngine;

namespace BebiLibs
{
    public class AndroidNotificationPublisher : INotificationPublisher
    {
        public string channelID = "channel1";
        public string channelName = "MainChannel";
        public string channelDescription = "Main Channel";
        override public void Init(MonoBehaviour parent)
        {
            AndroidNotificationChannel mainChannel = new AndroidNotificationChannel()
            {
                Id = this.channelID,
                Name = this.channelName,
                Importance = Importance.High,
                Description = this.channelDescription,
            };

            AndroidNotificationCenter.RegisterNotificationChannel(mainChannel);

            this.CallBackValid?.Invoke(true);
        }

        public override void RevokeAllLocalNotifications()
        {
            AndroidNotificationCenter.CancelAllNotifications();
        }

        public override void PublishLocalNotification(GameNotification gameNotification)
        {
            AndroidNotification notification;
            if (gameNotification.canRepeat)
            {
                notification = new AndroidNotification(gameNotification.title, gameNotification.content, gameNotification.notificationTime, gameNotification.repeatInterval)
                {
                    SmallIcon = gameNotification.smallIcon,
                    LargeIcon = gameNotification.largeIcon,
                    IntentData = gameNotification.data,
                };
            }
            else
            {
                notification = new AndroidNotification
                {
                    Title = gameNotification.title,
                    Text = gameNotification.content,
                    SmallIcon = gameNotification.smallIcon,
                    LargeIcon = gameNotification.largeIcon,
                    IntentData = gameNotification.data,
                    FireTime = gameNotification.notificationTime,
                };
            }

            AndroidNotificationCenter.SendNotification(notification, this.channelID);
        }

        public override string GetNotificationData()
        {
            var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
            if (notificationIntentData != null)
            {
                //int id = notificationIntentData.Id;
                //string channel = notificationIntentData.Channel;
                var notification = notificationIntentData.Notification;
                return notification.IntentData;
            }
            else
            {
                return null;
            }
        }
    }
}
#endif