using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{

    public class RandomFloatVariable : RandomVariable<float>
    {
        public RandomFloatVariable(string parameterID, float defaultValue, List<RandomValue<float>> keyValuePairs) : base(parameterID, defaultValue, keyValuePairs)
        {
            this.valueSetter = PlayerPrefs.SetFloat;
            this.valueGetter = PlayerPrefs.GetFloat;
        }

        public override string ToString()
        {
            return this.GetValue().ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}

