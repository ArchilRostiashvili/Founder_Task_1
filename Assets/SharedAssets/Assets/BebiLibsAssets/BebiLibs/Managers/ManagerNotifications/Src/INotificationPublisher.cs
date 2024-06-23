using System;
using UnityEngine;

namespace BebiLibs
{
    public class INotificationPublisher
    {
        public Action<bool> CallBackValid;

        /// <summary>
        /// Creates local notification
        /// Notification is fired on desired time
        /// </summary>
        /// <param name="when">when to fire a notification</param>
        /// <param name="title">notification title</param>
        /// <param name="content">notification content</param>
        /// <param name="sound">notification sound name</param>
        /// <param name="badge">number in badge</param>
        /// <param name="icon">notification icon name</param>
        /// <param name="picture">notification picture name</param>
        /// <returns>returns notification id</returns>
        /// 

        virtual public void Init(MonoBehaviour parent)
        {

        }

        public virtual void PublishLocalNotification(GameNotification gameNotification)
        {

        }


        public virtual string GetNotificationData()
        {
            return null;
        }

        /// <summary>
        /// Cancels local notification
        /// </summary>
        /// <param name="id">notification id</param>
        public virtual void RevokeLocalNotification(int id)
        {

        }

        /// <summary>
        /// Cancels all local notifications at once
        /// </summary>
        public virtual void RevokeAllLocalNotifications()
        {

        }
    }
}