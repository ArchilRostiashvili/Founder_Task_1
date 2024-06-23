using System;
using UnityEngine;

namespace BebiLibs
{
    public class TestNotificationPublisher : INotificationPublisher
    {
        public override void PublishLocalNotification(GameNotification gameNotification)
        {
            Common.DebugLog($"Publishing notification [{gameNotification.notificationTime} : {gameNotification.title} - {gameNotification.content}");
        }

        public override void RevokeLocalNotification(int id)
        {
            Common.DebugLog($"Revoking notification [{id}]");
        }

        public override void RevokeAllLocalNotifications()
        {
            Common.DebugLog("Revoking all notifications");
        }
    }
}