using BebiLibs.ABTestingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BebiLibs
{
    public class ExperimentEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text _experimentNameText;
        [SerializeField] private TMP_Dropdown _experimentDropDown;

        [SerializeField] List<string> options = new List<string>();

        public void DrawExperiment(Experiment objectToDraw)
        {
            _experimentNameText.text = objectToDraw.ExperimentName;
            List<Variant> variants = objectToDraw.VariantList;
            int selectedIndex = variants.IndexOf(objectToDraw.ActiveBaseVariant);

            foreach (var item in variants)
            {
                options.Add(item.VariantID);
            }

            _experimentDropDown.AddOptions(options);
            _experimentDropDown.value = selectedIndex;

            _experimentDropDown.onValueChanged.AddListener((int index) =>
            {
                PlayerPrefs.SetInt(objectToDraw.ExperimentName, index);
                PlayerPrefs.DeleteKey(objectToDraw.ExperimentName + "_activation_state");
                PlayerPrefs.Save();
            });
        }
    }
}
