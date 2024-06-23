using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ABTestingSystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "FloatExperiment", menuName = "BebiLibs/ABTestingSystem/FloatExperiment", order = 0)]
    public class FloatExperiment : Experiment<FloatVariant>
    {

    }

    [System.Serializable]
    public class FloatVariant : Variant
    {
        [SerializeField] private float _value;
        public float Value => _value;
        public FloatVariant(float probability, string name = "default") : base(probability, name)
        {
        }
    }
}
