using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class RandomBool : RandomVariable<bool>
    {
        public RandomBool(string parameterID, bool defaultValue, RandomValue<bool> valueOne, RandomValue<bool> valueTwo) : base(parameterID, defaultValue, new List<RandomValue<bool>> { valueOne, valueTwo })
        {
            valueGetter = GetBool;
            valueSetter = SetBool;
        }

        public void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, BoolToInt(value));
        }

        public bool GetBool(string key, bool defaultBool)
        {
            return PlayerPrefs.GetInt(key, BoolToInt(default)) == 1;
        }
    }
}
