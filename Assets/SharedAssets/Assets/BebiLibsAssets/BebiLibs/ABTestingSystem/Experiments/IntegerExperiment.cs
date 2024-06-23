using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ABTestingSystem
{
    [CreateAssetMenu(fileName = "IntegerExperiment", menuName = "BebiLibs/ABTestingSystem/IntegerExperiment", order = 0)]
    public class IntegerExperiment : Experiment<IntVariant>
    {

    }

    [System.Serializable]
    public class IntVariant : Variant
    {
        [SerializeField] private int _value;
        public int Value => _value;
        public IntVariant(float probability, string name = "default") : base(probability, name)
        {
        }
    }
}
