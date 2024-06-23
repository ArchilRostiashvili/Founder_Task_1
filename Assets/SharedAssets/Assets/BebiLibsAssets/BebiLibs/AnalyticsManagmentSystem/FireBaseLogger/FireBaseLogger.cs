using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs.Analytics.GameEventLogger;
using System.Linq;

#if ACTIVATE_ANALYTICS
using BebiLibs.Analytics;
using Firebase.Analytics;
using Firebase;
#endif

namespace BebiLibs.Analytics
{
    public class FireBaseLogger : LoggerBase
    {
        [Range(20, 100)]
        [SerializeField] private int _maxStoredEventCount = 100;

        private FirebaseParameterBuilder _parameterBuilder;
        private Queue<IGameEvent> _gameEvents = new Queue<IGameEvent>();

        private bool _isFirebaseAvailable = false;

#if ACTIVATE_ANALYTICS
        private void Awake()
        {
            FirebaseDependencyResolver.AddInitializationListener(StartLogging, true);
        }

        private void OnDestroy()
        {
            FirebaseDependencyResolver.RemoveInitializationListener(StartLogging);
        }

        private void StartLogging(bool isFirebaseAvailable)
        {
            _isFirebaseAvailable = isFirebaseAvailable;
            if (isFirebaseAvailable)
            {
                _parameterBuilder = new FirebaseParameterBuilder();
                InvokeRepeating(nameof(InvokeLog), 0.0f, Time.deltaTime);
            }
        }

        private void InvokeLog()
        {
            if (_gameEvents.Count > 0)
            {
                IGameEvent gameEvent = _gameEvents.Dequeue();
                gameEvent.InvokeLog(this);
                DisableInvokeLogMethod();
            }
            else
            {
                CancelInvoke(nameof(InvokeLog));
            }
        }

        private void DisableInvokeLogMethod()
        {
            if (_gameEvents.Count == 0)
            {
                CancelInvoke(nameof(InvokeLog));
            }
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                CancelInvoke(nameof(InvokeLog));
            }
        }
#endif

        public override void RecordEvent(IGameEvent gameEvent)
        {
#if ACTIVATE_ANALYTICS
            if (_isFirebaseAvailable && _gameEvents.Count == 0)
            {
                gameEvent.InvokeLog(this);
            }
            else
            {
                _gameEvents.Enqueue(gameEvent);
                if (_gameEvents.Count > _maxStoredEventCount)
                {
                    _gameEvents.Dequeue();
                }
            }
#endif
        }


        public override void LogEvent(SimpleEvent simpleEvent)
        {
#if ACTIVATE_ANALYTICS
            try
            {
                FirebaseAnalytics.LogEvent(simpleEvent.EventName);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
#endif
        }


        public override void LogEvent(StringEvent stringEvent)
        {
#if ACTIVATE_ANALYTICS
            try
            {
                FirebaseAnalytics.LogEvent(stringEvent.EventName, stringEvent.ParameterName, stringEvent.Value);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
#endif
        }



        public override void LogEvent(LongEvent longEvent)
        {
#if ACTIVATE_ANALYTICS
            try
            {
                FirebaseAnalytics.LogEvent(longEvent.EventName, longEvent.ParameterName, longEvent.Value);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
#endif
        }


        public override void LogEvent(DoubleEvent doubleEvent)
        {
#if ACTIVATE_ANALYTICS
            try
            {
                FirebaseAnalytics.LogEvent(doubleEvent.EventName, doubleEvent.ParameterName, doubleEvent.Value);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
#endif
        }

        public override void LogEvent(MultiParameterEvent multiParameterEvent)
        {
#if ACTIVATE_ANALYTICS
            try
            {
                _parameterBuilder.Clear();
                List<IGameParameter> parameters = multiParameterEvent.ParametersList.GroupBy(x => x.GetParameterName()).Select(x => x.Last()).ToList();
                for (int i = 0; i < parameters.Count; i++)
                {
                    parameters[i].UserParameter(this);
                }

                FirebaseAnalytics.LogEvent(multiParameterEvent.EventName, _parameterBuilder);
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
#endif
        }


        public override void HandleParameter(StringParameter parameter)
        {
#if ACTIVATE_ANALYTICS
            _parameterBuilder.Add(parameter.ParameterName, parameter.Value);
#endif
        }

        public override void HandleParameter(LongParameter parameter)
        {
#if ACTIVATE_ANALYTICS
            _parameterBuilder.Add(parameter.ParameterName, parameter.Value);
#endif
        }

        public override void HandleParameter(DoubleParameter parameter)
        {
#if ACTIVATE_ANALYTICS
            _parameterBuilder.Add(parameter.ParameterName, parameter.Value);
#endif
        }

        public override void SetProperty(GameProperty property)
        {
#if ACTIVATE_ANALYTICS
            try
            {
                FirebaseAnalytics.SetUserProperty(property.PropertyName, property.Value);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
#endif
        }

    }
}
