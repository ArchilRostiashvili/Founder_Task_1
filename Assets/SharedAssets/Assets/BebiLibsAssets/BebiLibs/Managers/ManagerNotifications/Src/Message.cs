using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.Notification
{
    [System.Serializable]
    public class NotificationMessage
    {
        public string errorDescription;
        public string error;
        public string MessageId;
        public IDictionary<string, string> Data;
        public string From;

        public NotificationMessage(string messageId, IDictionary<string, string> data, string from)
        {
            this.MessageId = messageId;
            this.Data = data;
            this.From = from;
        }
    }
}
