using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    public class RandomIntVariable : RandomVariable<int>
    {
        public RandomIntVariable(string parameterID, int defaultValue, List<RandomValue<int>> keyValuePairs) : base(parameterID, defaultValue, keyValuePairs)
        {
            this.valueGetter = PlayerPrefs.GetInt;
            this.valueSetter = PlayerPrefs.SetInt;
        }
    }
}

