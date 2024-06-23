using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


#if ACTIVATE_ANALYTICS && ACTIVATE_MIXPANEL
using mixpanel;
#endif

namespace BebiLibs.Analytics.GameEventLogger
{
    public class MixPanelLogger : LoggerBase
    {
        // [Range(20, 100)]
        // [SerializeField] private int _maxStoredEventCount = 100;
        // private Queue<IGameEvent> _gameEvents = new Queue<IGameEvent>();

#if ACTIVATE_MIXPANEL
        private Value _parameterBuilder;
#endif

        public override void RecordEvent(IGameEvent simpleEvent)
        {
#if ACTIVATE_ANALYTICS && ACTIVATE_MIXPANEL
            try
            {
                simpleEvent.InvokeLog(this);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Mix Panel Logger Error. Error Message " + e);
            }
#endif
        }


        public override void LogEvent(SimpleEvent simpleEvent)
        {
#if ACTIVATE_ANALYTICS && ACTIVATE_MIXPANEL
            try
            {
                Mixpanel.Track(simpleEvent.EventName);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Mix Panel Logger Error. Error Message " + e);
            }

#endif
        }

        public override void LogEvent(StringEvent stringEvent)
        {
#if ACTIVATE_ANALYTICS && ACTIVATE_MIXPANEL
            try
            {
                var props = new Value(stringEvent.Value);
                Mixpanel.Track(stringEvent.EventName, stringEvent.ParameterName, props);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Mix Panel Logger Error. Error Message " + e);
            }
#endif
        }

        public override void LogEvent(LongEvent longEvent)
        {
#if ACTIVATE_ANALYTICS && ACTIVATE_MIXPANEL
            try
            {
                var props = new Value(longEvent.Value);
                Mixpanel.Track(longEvent.EventName, longEvent.ParameterName, props);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Mix Panel Logger Error. Error Message " + e);
            }
#endif
        }

        public override void LogEvent(DoubleEvent doubleEvent)
        {
#if ACTIVATE_ANALYTICS && ACTIVATE_MIXPANEL
            try
            {
                var props = new Value(doubleEvent.Value);
                Mixpanel.Track(doubleEvent.EventName, doubleEvent.ParameterName, props);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Mix Panel Logger Error. Error Message " + e);
            }
#endif
        }


        public override void SetProperty(GameProperty property)
        {
#if ACTIVATE_ANALYTICS && ACTIVATE_MIXPANEL
            try
            {
                Mixpanel.Register(property.PropertyName, property.Value);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Mix Panel Logger Error. Error Message " + e);
            }
#endif
        }

        public override void LogEvent(MultiParameterEvent multiParameterEvent)
        {
#if ACTIVATE_ANALYTICS && ACTIVATE_MIXPANEL
            try
            {
                _parameterBuilder = new Value();
                List<IGameParameter> parameters = multiParameterEvent.ParametersList.GroupBy(x => x.GetParameterName()).Select(x => x.Last()).ToList();
                for (int i = 0; i < parameters.Count; i++)
                {
                    parameters[i].UserParameter(this);
                }

                Mixpanel.Track(multiParameterEvent.EventName, _parameterBuilder);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Mix Panel Logger Error. Error Message " + e);
            }
#endif
        }


        public override void HandleParameter(StringParameter parameter)
        {
#if ACTIVATE_ANALYTICS && ACTIVATE_MIXPANEL
            try
            {
                _parameterBuilder[parameter.ParameterName] = parameter.Value;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Mix Panel Logger Error. Error Message " + e);
            }
#endif
        }

        public override void HandleParameter(LongParameter parameter)
        {
#if ACTIVATE_ANALYTICS && ACTIVATE_MIXPANEL
            try
            {
                _parameterBuilder[parameter.ParameterName] = parameter.Value;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Mix Panel Logger Error. Error Message " + e);
            }
#endif
        }

        public override void HandleParameter(DoubleParameter parameter)
        {
#if ACTIVATE_ANALYTICS && ACTIVATE_MIXPANEL
            try
            {
                _parameterBuilder[parameter.ParameterName] = parameter.Value;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Mix Panel Logger Error. Error Message " + e);
            }
#endif
        }

    }
}
