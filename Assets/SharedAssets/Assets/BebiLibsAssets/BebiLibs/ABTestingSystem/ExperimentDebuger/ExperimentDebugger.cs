using BebiLibs.ABTestingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class ExperimentDebugger : MonoBehaviour
    {
        public Transform ContentTransformTR;
        public ExperimentEntry EntryPrefab;

        private void Awake()
        {
            Experiment.OnExperimentRegister += OnABExperimentRegister;
        }

        public void OnABExperimentRegister(Experiment aBExperiment)
        {
            ExperimentEntry entry = GameObject.Instantiate(EntryPrefab, ContentTransformTR);
            entry.DrawExperiment(aBExperiment);
        }

    }
}
