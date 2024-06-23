using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ABTestingSystem
{
    [CreateAssetMenu(fileName = "StringExperiment", menuName = "BebiLibs/ABTestingSystem/StringExperiment", order = 0)]
    public class StringExperiment : Experiment<StringVariant>
    {

    }

    [System.Serializable]
    public class StringVariant : Variant
    {
        [SerializeField] private string _value;
        public string Value => _value;
        public StringVariant(float probability, string name = "default") : base(probability, name)
        {
        }
    }
}
