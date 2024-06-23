using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BebiLibs.ABTestingSystem
{
    public abstract class Experiment : ScriptableObject
    {
        public static event System.Action<Experiment> OnExperimentRegister;
        [SerializeField] protected string _experimentName;
        [System.NonSerialized] protected bool _isActiveVariantUpdated = false;

        public string ExperimentName => _experimentName;

        public abstract Variant ActiveBaseVariant { get; }
        public abstract List<Variant> VariantList { get; }

        protected void InvokeExperimentRegisteredEvent(Experiment experiment)
        {
            OnExperimentRegister?.Invoke(experiment);
        }
    }

    public class Experiment<T> : Experiment where T : Variant
    {
        [SerializeField] protected List<T> _variantList = new List<T>();
        [SerializeField] protected T _defaultExperiment;
        [System.NonSerialized] protected T _activeExperiment;


        public bool IsStarted => PlayerPrefs.HasKey(_experimentName);
        public T ActiveVariant => GetActiveVariant();
        public T DefaultVariant => _defaultExperiment;

        public override Variant ActiveBaseVariant => ActiveVariant;
        public override List<Variant> VariantList => _variantList.Cast<Variant>().ToList();

        protected void SetExperimentParameters(string experimentName, T defaultVariant)
        {
            _experimentName = experimentName;
            _defaultExperiment = defaultVariant;
        }

        public void AddVariant(T variant)
        {
            if (!_variantList.Contains(variant))
            {
                _variantList.Add(variant);
                SetVariantIndexes();
            }
        }

        public void RemoveVariant(T variant)
        {
            if (_variantList.Contains(variant))
            {
                _variantList.Remove(variant);
                SetVariantIndexes();
            }
        }

        public void Clear()
        {
            _variantList.Clear();
        }

        //this will automatically activate experiment
        protected T GetActiveVariant()
        {
            if (_isActiveVariantUpdated) return _activeExperiment;
            StartExperimentIfNotInitialized();
            SetVariantIndexes();
            int elementIndex = PlayerPrefs.GetInt(_experimentName, -1);

            if (elementIndex >= 0 && elementIndex < _variantList.Count)
            {
                _activeExperiment = _variantList[elementIndex];
            }
            else
            {
                _activeExperiment = _defaultExperiment;
            }

            _isActiveVariantUpdated = true;
            InvokeExperimentRegisteredEvent(this);
            return _activeExperiment;
        }

        private void SetVariantIndexes()
        {
            for (int i = 0; i < _variantList.Count; i++)
            {
                _variantList[i].SetIndex(i);
            }
        }

        private void StartExperimentIfNotInitialized()
        {
            if (!PlayerPrefs.HasKey(_experimentName))
            {
                StartExperiment();
            }
        }

        private void StartExperiment()
        {
            T distributedFloat = GetRandom(_variantList, out int index);
            PlayerPrefs.SetInt(_experimentName, index);
            _activeExperiment = distributedFloat;
            _isActiveVariantUpdated = true;
        }

        protected T GetRandom(List<T> arrayInts, out int index)
        {
            index = 0;
            if (arrayInts.Count == 0) return _defaultExperiment;

            float total = 0;
            foreach (var elem in arrayInts)
            {
                total += elem.Probability;
            }

            float randomPoint = Random.value * total;

            for (int i = 0; i < arrayInts.Count; i++)
            {
                if (randomPoint < arrayInts[i].Probability)
                {
                    index = i;
                    return arrayInts[index];
                }
                else
                {
                    randomPoint -= arrayInts[i].Probability;
                }
            }
            index = arrayInts.Count - 1;
            return arrayInts[index];
        }
    }

}
