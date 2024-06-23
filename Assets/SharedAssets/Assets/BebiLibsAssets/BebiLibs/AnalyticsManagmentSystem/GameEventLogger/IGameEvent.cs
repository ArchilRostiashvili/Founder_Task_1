namespace BebiLibs.Analytics.GameEventLogger
{
    public interface IGameEvent
    {
        void InvokeLog(IEventLogger iEventLogger);
    };

}

