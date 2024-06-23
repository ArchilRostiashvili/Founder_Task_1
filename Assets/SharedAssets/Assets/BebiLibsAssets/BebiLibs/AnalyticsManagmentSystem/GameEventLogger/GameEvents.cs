using System;
using System.Collections.Generic;

namespace BebiLibs.Analytics.GameEventLogger
{
    public class GameProperty : IGameEvent
    {
        public string PropertyName => _isFromFunction ? _propertyNameFunc() : _propertyName;

        private string _propertyName;
        private Func<string> _propertyNameFunc;
        private bool _isFromFunction = false;

        public string Value;

        public GameProperty(string propertyName, string value)
        {
            _propertyName = propertyName;
            _isFromFunction = false;
            Value = value;
        }

        public GameProperty(Func<string> propertyNameFunc, string value)
        {
            _propertyNameFunc = propertyNameFunc;
            _isFromFunction = true;
            Value = value;
        }

        public void InvokeLog(IEventLogger iEventLogger)
        {
            iEventLogger.SetProperty(this);
        }
    }

    public class SimpleEvent : IGameEvent
    {
        public string EventName => _isEventNameSetToFunction ? _eventNameFunc() : _eventName;

        private string _eventName;
        private Func<string> _eventNameFunc;
        private bool _isEventNameSetToFunction = false;

        public SimpleEvent(string eventNameParam)
        {
            _eventName = LoggerNameValidator.ValidateName(eventNameParam);
            _isEventNameSetToFunction = false;
        }

        public SimpleEvent(Func<string> eventName)
        {
            _eventNameFunc = eventName;
            _isEventNameSetToFunction = true;
        }

        public virtual void InvokeLog(IEventLogger iEventLogger)
        {
            iEventLogger.LogEvent(this);
        }
    }

    public abstract class SimpleParameterEvent<T> : SimpleEvent
    {
        private string _parameterNameString;
        private Func<string> _parameterNameFunc;
        private bool _isParameterSetToFunction = false;

        public T Value;
        public string ParameterName => _isParameterSetToFunction ? _parameterNameFunc() : _parameterNameString;

        public SimpleParameterEvent(string eventName, string parameterName, T value) : base(eventName)
        {
            _parameterNameString = parameterName;
            _isParameterSetToFunction = false;
            Value = value;
        }

        public SimpleParameterEvent(Func<string> eventName, string parameterName, T value) : base(eventName)
        {
            _parameterNameString = parameterName;
            _isParameterSetToFunction = false;
            Value = value;
        }

        public SimpleParameterEvent(string eventName, Func<string> parameterName, T value) : base(eventName)
        {
            _parameterNameFunc = parameterName;
            _isParameterSetToFunction = true;
            Value = value;
        }

        public SimpleParameterEvent(Func<string> eventName, Func<string> parameterName, T value) : base(eventName)
        {
            _parameterNameFunc = parameterName;
            _isParameterSetToFunction = true;
            Value = value;
        }
    }

    public class StringEvent : SimpleParameterEvent<string>
    {
        public StringEvent(string eventName, string parameterName, string value) : base(eventName, parameterName, value)
        {
        }
        public StringEvent(Func<string> eventName, string parameterName, string value) : base(eventName, parameterName, value)
        {
        }
        public StringEvent(string eventName, Func<string> parameterName, string value) : base(eventName, parameterName, value)
        {
        }
        public StringEvent(Func<string> eventName, Func<string> parameterName, string value) : base(eventName, parameterName, value)
        {
        }
        public override void InvokeLog(IEventLogger iEventLogger)
        {
            iEventLogger.LogEvent(this);
        }
    }

    public class LongEvent : SimpleParameterEvent<long>
    {
        public LongEvent(string eventName, string parameterName, long value) : base(eventName, parameterName, value)
        {
        }
        public LongEvent(Func<string> eventName, string parameterName, long value) : base(eventName, parameterName, value)
        {
        }
        public LongEvent(string eventName, Func<string> parameterName, long value) : base(eventName, parameterName, value)
        {
        }
        public LongEvent(Func<string> eventName, Func<string> parameterName, long value) : base(eventName, parameterName, value)
        {
        }
        public override void InvokeLog(IEventLogger iEventLogger)
        {
            iEventLogger.LogEvent(this);
        }
    }

    public class DoubleEvent : SimpleParameterEvent<double>
    {
        public DoubleEvent(string eventName, string parameterName, double value) : base(eventName, parameterName, value)
        {
        }
        public DoubleEvent(Func<string> eventName, string parameterName, double value) : base(eventName, parameterName, value)
        {
        }
        public DoubleEvent(string eventName, Func<string> parameterName, double value) : base(eventName, parameterName, value)
        {
        }
        public DoubleEvent(Func<string> eventName, Func<string> parameterName, double value) : base(eventName, parameterName, value)
        {
        }
        public override void InvokeLog(IEventLogger iEventLogger)
        {
            iEventLogger.LogEvent(this);
        }
    }

    public class MultiParameterEvent : SimpleEvent
    {
        private GameParameterBuilder _parameterBuilder = new GameParameterBuilder();

        public GameParameterBuilder ParameterBuilder => _parameterBuilder;
        public List<IGameParameter> ParametersList => _parameterBuilder.GetParameters();

        public MultiParameterEvent(string eventName) : base(eventName)
        {

        }

        public MultiParameterEvent(Func<string> eventName) : base(eventName)
        {

        }

        public MultiParameterEvent(string eventName, List<IGameParameter> parameters) : base(eventName)
        {
            _parameterBuilder.Add(parameters);
        }

        public MultiParameterEvent(Func<string> eventName, List<IGameParameter> parameters) : base(eventName)
        {
            _parameterBuilder.Add(parameters);
        }

        public override void InvokeLog(IEventLogger iEventLogger)
        {
            iEventLogger.LogEvent(this);
        }
    }
}