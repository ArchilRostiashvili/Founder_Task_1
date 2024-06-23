using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    [System.Serializable]
    public class GameNotification
    {
        public DateTime notificationTime;
        public TimeSpan repeatInterval = TimeSpan.Zero;
        public bool canRepeat;
        public string title = string.Empty;
        public string content = string.Empty;
        public string data = string.Empty;
        public string smallIcon = "notification_small";
        public string largeIcon = "notification_large";
        public string sound = "notification_sound";
        public int badge = 0;
    }
}
