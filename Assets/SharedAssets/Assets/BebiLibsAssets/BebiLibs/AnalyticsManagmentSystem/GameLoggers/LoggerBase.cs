using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs.Analytics.GameEventLogger;

namespace BebiLibs.Analytics
{
    public abstract class LoggerBase : MonoBehaviour, IEventLogger
    {
        public abstract void HandleParameter(StringParameter parameter);
        public abstract void HandleParameter(LongParameter parameter);
        public abstract void HandleParameter(DoubleParameter parameter);
        public abstract void LogEvent(SimpleEvent simpleEvent);
        public abstract void LogEvent(StringEvent stringEvent);
        public abstract void LogEvent(LongEvent stringEvent);
        public abstract void LogEvent(DoubleEvent stringEvent);
        public abstract void LogEvent(MultiParameterEvent multiParameterEvent);
        public abstract void RecordEvent(IGameEvent simpleEvent);
        public abstract void SetProperty(GameProperty property);
    }
}
