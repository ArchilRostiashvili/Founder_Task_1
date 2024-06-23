using BebiLibs.Analytics.GameEventLogger;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace BebiLibs.Analytics
{
    public class AnalyticsManager : GenericSingletonClass<AnalyticsManager>
    {
        private static List<IEventLogger> _EventLoggersList = new List<IEventLogger>();
        private static List<IParameterProvider> _DefaultAnalyticsProviderList = new List<IParameterProvider>();

        [SerializeField] private List<LoggerBase> _loggerBaseList = new List<LoggerBase>();

        protected override void OnInstanceAwake()
        {
            for (int i = 0; i < _loggerBaseList.Count; i++)
            {
                RegisterEventListener(_loggerBaseList[i]);
            }
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void RegisterEventListener(IEventLogger iEventLogger)
        {
            if (!_EventLoggersList.Contains(iEventLogger))
            {
                _EventLoggersList.Add(iEventLogger);
            }
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void UnregisterEventListener(IEventLogger iEventLogger)
        {
            if (_EventLoggersList.Contains(iEventLogger))
            {
                _EventLoggersList.Remove(iEventLogger);
            }
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void RegisterParameterProvider(IParameterProvider parameterProvider)
        {
            if (!_DefaultAnalyticsProviderList.Contains(parameterProvider))
            {
                _DefaultAnalyticsProviderList.Add(parameterProvider);
            }
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void UnregisterParameterProvider(IParameterProvider parameterProvider)
        {
            if (_DefaultAnalyticsProviderList.Contains(parameterProvider))
            {
                _DefaultAnalyticsProviderList.Remove(parameterProvider);
            }
        }


        private static GameParameterBuilder AddDefaultParameters(GameParameterBuilder parameterBuilder)
        {
#if ACTIVATE_ANALYTICS
            foreach (IParameterProvider item in _DefaultAnalyticsProviderList)
            {
                parameterBuilder.Add(item.GetParameters());
            }
            return parameterBuilder;
#else
            return null;
#endif
        }

        private static void RecordMultiParameterEvent(MultiParameterEvent gameEvent)
        {
#if ACTIVATE_ANALYTICS
            foreach (IEventLogger eventLogger in _EventLoggersList)
            {
                eventLogger.RecordEvent(gameEvent);
            }
#endif
        }




        [Conditional("ACTIVATE_ANALYTICS")]
        public static void LogEvent(string eventName)
        {
            MultiParameterEvent gameEvent = new MultiParameterEvent(eventName);
            AddDefaultParameters(gameEvent.ParameterBuilder);
            RecordMultiParameterEvent(gameEvent);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        public static void LogEvent(System.Func<string> eventNameFunc)
        {
            MultiParameterEvent gameEvent = new MultiParameterEvent(eventNameFunc);
            AddDefaultParameters(gameEvent.ParameterBuilder);
            RecordMultiParameterEvent(gameEvent);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        public static void LogEvent(string eventName, string parameterName, string value)
        {
            MultiParameterEvent gameEvent = new MultiParameterEvent(eventName);
            gameEvent.ParameterBuilder.Add(parameterName, value);
            AddDefaultParameters(gameEvent.ParameterBuilder);
            RecordMultiParameterEvent(gameEvent);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        public static void LogEvent(System.Func<string> eventNameFunc, string parameterName, string value)
        {
            MultiParameterEvent gameEvent = new MultiParameterEvent(eventNameFunc);
            gameEvent.ParameterBuilder.Add(parameterName, value);
            AddDefaultParameters(gameEvent.ParameterBuilder);
            RecordMultiParameterEvent(gameEvent);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        public static void LogEvent(string eventName, string parameterName, long value)
        {
            MultiParameterEvent gameEvent = new MultiParameterEvent(eventName);
            gameEvent.ParameterBuilder.Add(parameterName, value);
            AddDefaultParameters(gameEvent.ParameterBuilder);
            RecordMultiParameterEvent(gameEvent);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        public static void LogEvent(string eventName, string parameterName, double value)
        {
            MultiParameterEvent gameEvent = new MultiParameterEvent(eventName);
            gameEvent.ParameterBuilder.Add(parameterName, value);
            AddDefaultParameters(gameEvent.ParameterBuilder);
            RecordMultiParameterEvent(gameEvent);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        public static void LogEvent(string eventName, List<IGameParameter> parameters)
        {
            MultiParameterEvent gameEvent = new MultiParameterEvent(eventName);
            gameEvent.ParameterBuilder.Add(parameters);
            AddDefaultParameters(gameEvent.ParameterBuilder);
            RecordMultiParameterEvent(gameEvent);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        public static void SetProperty(string propertyName, string value)
        {
            GameProperty property = new GameProperty(propertyName, value);
            _EventLoggersList.ForEach(x => x.RecordEvent(property));
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void RecordEvent(IGameEvent gameEvent)
        {
            _EventLoggersList.ForEach(x => x.RecordEvent(gameEvent));
        }

    }
}