using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class RandomString : RandomVariable<string>
    {
        public RandomString(string parameterID, string defaultValue, List<RandomValue<string>> keyValuePairs) : base(parameterID, defaultValue, keyValuePairs)
        {
            this.valueGetter = PlayerPrefs.GetString;
            this.valueSetter = PlayerPrefs.SetString;
        }
    }
}
