using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Collections;
using BebiLibs.Analytics.GameEventLogger;
using System.Linq;

namespace BebiLibs.Analytics
{
    public class SimpleLogger : LoggerBase
    {

        [SerializeField] private bool _enableLogging = false;
        private Queue<IGameEvent> _gameEvents = new Queue<IGameEvent>();
        private StringBuilder _parameterBuilder = new StringBuilder();


        private void OnEnable()
        {
            if (_enableLogging && Application.isPlaying)
            {
                InvokeRepeating("InvokeLog", 0.0f, 0.05f);
            }
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                CancelInvoke("InvokeLog");
            }
        }

        private void InvokeLog()
        {
            if (_gameEvents.Count > 0)
            {
                IGameEvent gameEvent = _gameEvents.Dequeue();
                gameEvent.InvokeLog(this);
            }
        }

        public override void RecordEvent(IGameEvent gameEvent)
        {
            if (_enableLogging)
            {
                _gameEvents.Enqueue(gameEvent);
            }
        }

        public override void LogEvent(SimpleEvent simpleEvent)
        {
            Debug.LogError($"Event: {simpleEvent.EventName}");
        }

        public override void LogEvent(StringEvent stringEvent)
        {
            Debug.LogError($"Event: {stringEvent.EventName}, Parameter: {stringEvent.ParameterName}, Value: {stringEvent.Value}");
        }

        public override void LogEvent(LongEvent stringEvent)
        {
            Debug.LogError($"Event: {stringEvent.EventName}, Parameter: {stringEvent.ParameterName}, Value: {stringEvent.Value}");
        }

        public override void LogEvent(DoubleEvent stringEvent)
        {
            Debug.LogError($"Event: {stringEvent.EventName}, Parameter: {stringEvent.ParameterName}, Value: {stringEvent.Value}");
        }

        public override void LogEvent(MultiParameterEvent multiParameterEvent)
        {
            _parameterBuilder.Clear();
            List<IGameParameter> parameters = multiParameterEvent.ParametersList.GroupBy(x => x.GetParameterName()).Select(x => x.Last()).ToList();
            for (int i = 0; i < parameters.Count; i++)
            {
                parameters[i].UserParameter(this);
            }

            Debug.LogError($"Event: {multiParameterEvent.EventName},\n{_parameterBuilder}");
        }

        public override void SetProperty(GameProperty property)
        {
            Debug.LogError($"Set Property: {property.PropertyName}, value {property.Value}");
        }

        public override void HandleParameter(StringParameter parameter)
        {
            _parameterBuilder.AppendLine($"    Parameter: {parameter.ParameterName}, Value: {parameter.Value}");
        }

        public override void HandleParameter(LongParameter parameter)
        {
            _parameterBuilder.AppendLine($"    Parameter: {parameter.ParameterName}, Value: {parameter.Value}");
        }

        public override void HandleParameter(DoubleParameter parameter)
        {
            _parameterBuilder.AppendLine($"    Parameter: {parameter.ParameterName}, Value: {parameter.Value}");
        }
    }
}