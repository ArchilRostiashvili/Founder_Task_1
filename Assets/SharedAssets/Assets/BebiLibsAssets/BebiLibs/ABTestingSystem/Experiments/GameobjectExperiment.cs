using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ABTestingSystem
{
    [CreateAssetMenu(fileName = "GameObjectExperiment", menuName = "BebiLibs/ABTestingSystem/GameObjectExperiment", order = 0)]
    public class GameObjectExperiment : Experiment<GameObjectVariant>
    {

    }

    [System.Serializable]
    public class GameObjectVariant : Variant
    {
        [SerializeField] private GameObject _valueGO;
        public GameObject Value => _valueGO;
        public GameObjectVariant(float probability, string name = "default") : base(probability, name)
        {
        }

    }


}
