using BebiLibs.Localization.Core;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.Localization
{
    [RequireComponent(typeof(Transform))]
    public class TransformLocalization : BaseLocalizationComponent
    {
        [System.Serializable]
        public class DataTransformLocalization : BaseLanguageProperties
        {
            public Vector3 localPosition;
            public Vector3 rotation;
            public Vector2 localScale;
        }

        [SerializeField] private List<DataTransformLocalization> _languageData = new List<DataTransformLocalization>();

        public override BaseLanguageProperties this[int index] => _languageData[index];
        public override int dataCount => _languageData.Count;
        public override int activeIndex => _languageData.FindIndex(x => x.languageKey == LocalizationManager.ActiveLanguage.LanguageName);

        private Transform _transform;

        public override void Init()
        {
            _transform = this.GetComponent<Transform>();
        }

        public override void UpdateComponent(LanguageIdentifier languageIdentifier)
        {
            if (_transform == null) return;
            DataTransformLocalization property = _languageData.Find(x => x.languageKey == languageIdentifier.LanguageName);

            if (property == null) return;
            _transform.localPosition = property.localPosition;
            _transform.localScale = property.localScale;
            _transform.rotation = Quaternion.Euler(property.rotation);
        }


        public override bool IsChanged()
        {
            if (_transform == null) return false;
            DataTransformLocalization property = _languageData.Find(x => x.languageKey == LocalizationManager.ActiveLanguage.LanguageName);

            if (property == null) return false;

            bool local = _transform.localPosition != property.localPosition;
            bool scale = (Vector2)_transform.localScale != property.localScale;
            bool rotation = _transform.rotation != Quaternion.Euler(property.rotation);

            return local || scale || rotation;
        }

        public override void LoadDefaults()
        {
            if (_transform == null) return;
            foreach (var dataLocal in _languageData)
            {
                this.SetDefault(dataLocal);
            }
        }


        public void SetDefault(DataTransformLocalization dataLocal)
        {
            if (dataLocal != null && _transform != null)
            {
                dataLocal.localPosition = _transform.localPosition;
                dataLocal.localScale = _transform.localScale;
                dataLocal.rotation = _transform.rotation.eulerAngles;
            }
        }

        public override void LoadDefaultToActiveLanguage(string code)
        {
            base.ThrowDisabledExertion();

            foreach (var dataLang in _languageData)
            {
                if (dataLang.languageKey == code)
                {
                    this.SetDefault(dataLang);
                }
            }
        }

        public override void AddMissingLanguages()
        {
            base.ThrowDisabledExertion();

            List<string> localLanguages = I2.Loc.LocalizationManager.GetAllLanguages();

            foreach (string code in localLanguages)
            {
                if (_languageData.Find(x => x.languageKey == code) == null)
                {
                    _languageData.Add(new DataTransformLocalization()
                    {
                        languageKey = code
                    });
                    this.LoadDefaultToActiveLanguage(code);
                }
            }
        }

        public override void RetrieveAllLanguages()
        {
            _languageData.Clear();
            List<string> localLanguages = I2.Loc.LocalizationManager.GetAllLanguages();
            foreach (string code in localLanguages)
            {
                if (_languageData.Find(x => x.languageKey == code) == null)
                {
                    _languageData.Add(new DataTransformLocalization()
                    {
                        languageKey = code
                    });
                }
            }
            this.Init();
            this.LoadDefaults();
        }
    }
}
