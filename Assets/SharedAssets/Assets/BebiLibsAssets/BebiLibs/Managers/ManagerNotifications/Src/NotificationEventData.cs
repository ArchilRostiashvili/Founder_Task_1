namespace BebiLibs.Notification
{
    [System.Serializable]
    public struct NotificationEventData
    {
        public NotificationSource source;
        public bool isPublisherAvailable;
        public string notificationData;

        public NotificationEventData(NotificationSource source, bool isPublisherAvailable, string notificationData)
        {
            this.source = source;
            this.isPublisherAvailable = isPublisherAvailable;
            this.notificationData = notificationData;
        }
    }


    public enum NotificationSource
    {
        Initialization, UserInteraction
    }
}
