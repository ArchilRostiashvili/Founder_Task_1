using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ACTIVATE_ANALYTICS
using Firebase;
using Firebase.Analytics;
#endif

namespace BebiLibs.Analytics
{
    public class FirebaseParameterBuilder
    {
#if ACTIVATE_ANALYTICS
        private List<Parameter> _arrayParameter;


        public FirebaseParameterBuilder()
        {
            _arrayParameter = new List<Parameter>();
        }

        public FirebaseParameterBuilder Add(string parameterName, string parameterValue)
        {
            _arrayParameter.Add(new Parameter(parameterName, parameterValue));
            return this;
        }
        public FirebaseParameterBuilder Add(string parameterName, long parameterValue)
        {
            _arrayParameter.Add(new Parameter(parameterName, parameterValue));
            return this;
        }
        public FirebaseParameterBuilder Add(string parameterName, double parameterValue)
        {
            _arrayParameter.Add(new Parameter(parameterName, parameterValue));
            return this;
        }

        public static implicit operator Parameter[](FirebaseParameterBuilder d) => d.ToArray();

        public Parameter[] ToArray()
        {
            return _arrayParameter.ToArray();
        }

        public FirebaseParameterBuilder Clear()
        {
            _arrayParameter.Clear();
            return this;
        }
#endif
    }
}
