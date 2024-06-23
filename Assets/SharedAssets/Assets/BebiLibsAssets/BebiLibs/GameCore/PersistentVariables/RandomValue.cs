//using UnityEngine;

namespace BebiLibs
{
    public class RandomValue<T>
    {
        public T value;
        public float probability;

        public RandomValue(T value, float probability)
        {
            this.value = value;
            this.probability = probability;
        }
    }
}


