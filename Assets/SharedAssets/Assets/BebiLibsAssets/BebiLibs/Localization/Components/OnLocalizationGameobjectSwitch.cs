using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs.Localization.Core;

namespace BebiLibs.Localization
{
    [ExecuteInEditMode]
    public class OnLocalizationGameobjectSwitch : MonoBehaviour
    {
        public enum WorkMode
        {
            Activate, Deactivate
        };

        public WorkMode matchLanguage;
        public WorkMode notMatchLanguage = WorkMode.Activate;

        public List<LanguageIdentifier> arrayAcceptedTypeFilter = new List<LanguageIdentifier>();
        public List<GameObject> arrayGameObjectsToOperate = new List<GameObject>();


        public void UpdateState(LanguageIdentifier languageIdentifier)
        {
            if (arrayAcceptedTypeFilter.Contains(languageIdentifier))
            {
                for (int i = 0; i < arrayGameObjectsToOperate.Count; i++)
                {
                    arrayGameObjectsToOperate[i].SetActive(matchLanguage == WorkMode.Activate);
                }
            }
            else
            {
                for (int i = 0; i < arrayGameObjectsToOperate.Count; i++)
                {
                    arrayGameObjectsToOperate[i].SetActive(notMatchLanguage == WorkMode.Activate);
                }
            }
        }


        private void OnEnable()
        {
            LocalizationManager.OnLanguageChangeEvent -= UpdateState;
            LocalizationManager.OnLanguageChangeEvent += UpdateState;
            UpdateState(LocalizationManager.ActiveLanguage);
        }

        private void OnDisable()
        {
            LocalizationManager.OnLanguageChangeEvent -= UpdateState;
        }
    }
}
