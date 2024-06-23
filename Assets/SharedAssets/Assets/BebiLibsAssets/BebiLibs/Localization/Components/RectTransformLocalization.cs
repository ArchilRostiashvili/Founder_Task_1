using BebiLibs.Localization.Core;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.Localization
{
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformLocalization : BaseLocalizationComponent
    {
        [System.Serializable]
        public class DataRectLocalization : BaseLanguageProperties
        {
            public Vector2 anchorMin;
            public Vector2 anchorMax;
            public Vector2 anchoredPosition;
            public Vector2 sizeDelta;
            public Vector2 pivot;
            public bool overrideLayoutControl = false;
        }


        [SerializeField] private List<DataRectLocalization> _languageData = new List<DataRectLocalization>();


        public override BaseLanguageProperties this[int index] => _languageData[index];
        public override int dataCount => _languageData.Count;
        public override int activeIndex => _languageData.FindIndex(x => x.languageKey == LocalizationManager.ActiveLanguage.LanguageName);

        private RectTransform _rectTransform;

        public override void Init()
        {
            _rectTransform = this.GetComponent<RectTransform>();
        }

        public override void UpdateComponent(LanguageIdentifier languageIdentifier)
        {
            if (_rectTransform == null) return;
            DataRectLocalization property = _languageData.Find(x => x.languageKey == languageIdentifier.LanguageName);

            if (property == null) return;

            _rectTransform.anchorMin = property.anchorMin;
            _rectTransform.anchorMax = property.anchorMax;
            _rectTransform.anchoredPosition = _isControlledByLayout && !property.overrideLayoutControl ? _rectTransform.anchoredPosition : property.anchoredPosition;
            _rectTransform.sizeDelta = property.sizeDelta;
            _rectTransform.pivot = property.pivot;
        }

        public override bool IsChanged()
        {
            if (_rectTransform == null) return false;
            DataRectLocalization property = _languageData.Find(x => x.languageKey == LocalizationManager.ActiveLanguage.LanguageName);

            if (property == null) return false;

            bool Min = _rectTransform.anchorMin != property.anchorMin;
            bool Max = _rectTransform.anchorMax != property.anchorMax;
            bool Pos = _rectTransform.anchoredPosition != property.anchoredPosition;
            bool size = _rectTransform.sizeDelta != property.sizeDelta;
            bool pivot = _rectTransform.pivot != property.pivot;

            return Min || Max || Pos || size || pivot;
        }

        private void Reset()
        {
            this.Init();
            this.LoadDefaults();
        }

        public void SetDefault(DataRectLocalization dataLocal)
        {
            dataLocal.anchorMin = _rectTransform.anchorMin;
            dataLocal.anchorMax = _rectTransform.anchorMax;
            dataLocal.anchoredPosition = _rectTransform.anchoredPosition;
            dataLocal.sizeDelta = _rectTransform.sizeDelta;
            dataLocal.pivot = _rectTransform.pivot;
        }

        public override void LoadDefaults()
        {
            if (_rectTransform == null) return;
            foreach (var lang in _languageData)
            {
                this.SetDefault(lang);
            }
        }


        public override void LoadDefaultToActiveLanguage(string code)
        {
            base.ThrowDisabledExertion();


            if (_rectTransform == null) return;
            foreach (var lang in _languageData)
            {
                if (lang.languageKey == code)
                {
                    this.SetDefault(lang);
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
                    _languageData.Add(new DataRectLocalization()
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
                    _languageData.Add(new DataRectLocalization()
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
