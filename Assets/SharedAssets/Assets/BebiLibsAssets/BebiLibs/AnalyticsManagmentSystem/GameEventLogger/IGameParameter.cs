namespace BebiLibs.Analytics.GameEventLogger
{
    public interface IGameParameter
    {
        string GetParameterName();
        void UserParameter(IEventLogger iEventLogger);
    };
}
