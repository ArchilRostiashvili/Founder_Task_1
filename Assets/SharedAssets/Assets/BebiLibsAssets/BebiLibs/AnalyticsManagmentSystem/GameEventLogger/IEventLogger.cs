namespace BebiLibs.Analytics.GameEventLogger
{
    public interface IEventLogger
    {
        void RecordEvent(IGameEvent simpleEvent);
        void LogEvent(SimpleEvent simpleEvent);
        void LogEvent(StringEvent stringEvent);
        void LogEvent(LongEvent stringEvent);
        void LogEvent(DoubleEvent stringEvent);
        void LogEvent(MultiParameterEvent multiParameterEvent);
        void SetProperty(GameProperty property);
        void HandleParameter(StringParameter parameter);
        void HandleParameter(LongParameter parameter);
        void HandleParameter(DoubleParameter parameter);
    }
}
