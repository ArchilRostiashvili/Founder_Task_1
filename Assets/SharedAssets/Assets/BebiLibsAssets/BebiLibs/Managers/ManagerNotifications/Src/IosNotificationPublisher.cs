#if UNITY_IOS
using System;
using System.Collections;
using Unity.Notifications.iOS;
using UnityEngine;


namespace BebiLibs
{
    public class IosNotificationPublisher : INotificationPublisher
    {
        override public void Init(MonoBehaviour parent)
        {
            _identifier = 0;
            parent.StartCoroutine(this.RequestAuthorization());
        }

        private IEnumerator RequestAuthorization()
        {
            using (var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true))
            {
                while (!req.IsFinished)
                {
                    yield return null;
                }

                string res = "\n RequestAuthorization: \n";
                res += "\n finished: " + req.IsFinished;
                res += "\n granted :  " + req.Granted;
                res += "\n error:  " + req.Error;
                res += "\n deviceToken:  " + req.DeviceToken;
                //Common.DebugLog(res);
                if (req.Granted)
                {
                    this.CallBackValid?.Invoke(true);
                }
                else
                {
                    this.CallBackValid?.Invoke(false);
                }
            }
        }

        public override void RevokeAllLocalNotifications()
        {
            iOSNotificationCenter.RemoveAllScheduledNotifications();
        }

        private int _identifier;

        public override void PublishLocalNotification(GameNotification gameNotification)
        {
            _identifier++;
            /*
            var calendarTrigger = new iOSNotificationCalendarTrigger()
            {
                // Year = 2018,
                // Month = 8,
                //Day = 30,
                Hour = when.Hour,
                Minute = when.Minute,
                // Second = 0
                Repeats = false
            };
            */


            DateTime dateTimeToday = DateTime.Now;
            TimeSpan timeSpan = gameNotification.notificationTime.Subtract(dateTimeToday);

            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = timeSpan,
                Repeats = false,

            };

            var notification = new iOSNotification()
            {
                Identifier = "notification_" + _identifier,
                Title = gameNotification.title,
                Body = gameNotification.content,
                Subtitle = "",
                ShowInForeground = false,
                //ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                CategoryIdentifier = "category_a",
                ThreadIdentifier = "thread1",
                Badge = gameNotification.badge,
                Trigger = timeTrigger,
                Data = gameNotification.data,
            };

            iOSNotificationCenter.ScheduleNotification(notification);
        }

        public override string GetNotificationData()
        {
            var n = iOSNotificationCenter.GetLastRespondedNotification();
            if (n != null)
            {
                /*
                var msg = "Last Received Notification : " + n.Identifier + "\n";
                msg += "\n - Notification received: ";
                msg += "\n - .Title: " + n.Title;
                msg += "\n - .Badge: " + n.Badge;
                msg += "\n - .Body: " + n.Body;
                msg += "\n - .CategoryIdentifier: " + n.CategoryIdentifier;
                msg += "\n - .Subtitle: " + n.Subtitle;
                msg += "\n - .Data: " + n.Data;
                Common.DebugLog(msg);
                */
                return n.Data;
            }
            else
            {
                //Common.DebugLog("No notifications received.");
                return null;
            }
        }
    }
}
#endif