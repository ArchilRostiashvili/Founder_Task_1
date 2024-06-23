using System.Collections;
using System.Collections.Generic;
//using UnityEngine;



namespace BebiLibs
{
    public class RandomVariable<T> : PersistentVariable
    {
        internal List<RandomValue<T>> _arrayDistribution = new List<RandomValue<T>>();
        public T _runtimeValue;

        public delegate T ValueGetter(string key, T defaultValue);
        public delegate void ValueSetter(string key, T newValue);

        public ValueGetter valueGetter { get; protected set; }
        public ValueSetter valueSetter { get; protected set; }

        protected RandomVariable(string parameterID, T defaultValue, List<RandomValue<T>> keyValuePairs) : base(parameterID)
        {
            _arrayDistribution = keyValuePairs;
            _runtimeValue = defaultValue;
        }

        public RandomVariable(string parameterID, T defaultValue, List<RandomValue<T>> keyValuePairs, ValueSetter valueSetter, ValueGetter valueGetter) : base(parameterID)
        {
            this.valueSetter = valueSetter;
            this.valueGetter = valueGetter;
            _arrayDistribution = keyValuePairs;
            _runtimeValue = defaultValue;
        }


        private RandomValue<T> GetRandom(List<RandomValue<T>> arrayInts)
        {
            if (arrayInts.Count == 0) return new RandomValue<T>(_runtimeValue, 0.5f);

            float total = 0;
            foreach (var elem in arrayInts)
            {
                total += elem.probability;
            }

            float randomPoint = UnityEngine.Random.value * total;

            for (int i = 0; i < arrayInts.Count; i++)
            {
                if (randomPoint < arrayInts[i].probability)
                {
                    return arrayInts[i];
                }
                else
                {
                    randomPoint -= arrayInts[i].probability;
                }
            }
            return arrayInts[arrayInts.Count - 1];
        }


        public T value
        {
            get => GetValue();
            private set => SetValue(value);
        }

        private void SetValue(T value)
        {
            if (!IsLocked)
            {
                _runtimeValue = value;
                valueSetter(_parameterID, _runtimeValue);
            }
        }

        public void InitializeDefault()
        {
            if (!isInitialized)
            {
                RandomValue<T> distributedFloat = GetRandom(_arrayDistribution);
                valueSetter(_parameterID, distributedFloat.value);
                _runtimeValue = distributedFloat.value;
            }
        }

        public T GetValue()
        {
            if (base._isValueUpdated) return _runtimeValue;
            InitializeDefault();
            _runtimeValue = valueGetter(_parameterID, _runtimeValue);
            base._isValueUpdated = true;
            return _runtimeValue;
        }

        public static implicit operator T(RandomVariable<T> d) => d.GetValue();

        public override string ToString()
        {
            return GetValue().ToString();
        }
    }
}

