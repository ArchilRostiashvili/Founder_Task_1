using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.Analytics.GameEventLogger
{
    public abstract class BaseParameter<T> : IGameParameter
    {
        private string _parameterNameString;
        private Func<string> _parameterNameFunc;
        private bool _isParameterSetToFunction = false;

        public T Value;
        public string ParameterName => _isParameterSetToFunction ? _parameterNameFunc() : _parameterNameString;

        public string GetParameterName() => ParameterName;

        public BaseParameter(string parameterName, T value)
        {
            _parameterNameString = LoggerNameValidator.ValidateName(parameterName);
            _isParameterSetToFunction = false;
            Value = value;
        }

        public BaseParameter(Func<string> parameterName, T value)
        {
            _parameterNameFunc = parameterName;
            _isParameterSetToFunction = true;
            Value = value;
        }

        public abstract void UserParameter(IEventLogger iEventLogger);
    }


    public class StringParameter : BaseParameter<string>
    {
        public StringParameter(string parameterName, string value) : base(parameterName, value)
        {
        }

        public StringParameter(Func<string> parameterName, string value) : base(parameterName, value)
        {
        }

        public override void UserParameter(IEventLogger iEventLogger)
        {
            iEventLogger.HandleParameter(this);
        }
    }

    public class LongParameter : BaseParameter<long>
    {
        public LongParameter(string parameterName, long value) : base(parameterName, value)
        {
        }

        public LongParameter(Func<string> parameterName, long value) : base(parameterName, value)
        {
        }

        public override void UserParameter(IEventLogger iEventLogger)
        {
            iEventLogger.HandleParameter(this);
        }
    }

    public class DoubleParameter : BaseParameter<double>
    {
        public DoubleParameter(string parameterName, double value) : base(parameterName, value)
        {
        }

        public DoubleParameter(Func<string> parameterName, double value) : base(parameterName, value)
        {
        }

        public override void UserParameter(IEventLogger iEventLogger)
        {
            iEventLogger.HandleParameter(this);
        }
    }
}