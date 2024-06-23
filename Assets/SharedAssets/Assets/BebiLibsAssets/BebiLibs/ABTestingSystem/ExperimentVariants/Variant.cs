using UnityEngine;

namespace BebiLibs.ABTestingSystem
{
    [System.Serializable]
    public abstract class Variant
    {
        [SerializeField] private string _name;
        [Range(0, 1)]
        [SerializeField] private float _probability;

        private int _index;

        public string VariantID => _name;
        public float Probability => _probability;
        public int Index => _index;

        public Variant(float probability, string name = "default")
        {
            _probability = probability;
            _name = name;
        }

        internal void SetIndex(int index)
        {
            _index = index;
        }

    }
}
