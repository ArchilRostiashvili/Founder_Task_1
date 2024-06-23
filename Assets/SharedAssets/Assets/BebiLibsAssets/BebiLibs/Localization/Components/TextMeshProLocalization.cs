using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BebiLibs.Localization.Core;

namespace BebiLibs.Localization
{
    public class TextMeshProLocalization : BaseLocalizationComponent
    {
        [System.Serializable]
        public class LanguageProperties : BaseLanguageProperties
        {
            public Material fontMaterial;
            public float fontSize;
            public float charSpace;
            public float lineSpace;
            public bool bold;
            public Vector2 pos;
            public Vector2 size;
        }

        [SerializeField] private List<LanguageProperties> _languageData = new List<LanguageProperties>();

        public override BaseLanguageProperties this[int index] => _languageData[index];
        public override int dataCount => _languageData.Count;

        public override int activeIndex => _languageData.FindIndex(x => x.languageKey == LocalizationManager.ActiveLanguage.LanguageName);

        private TMP_Text _tmpro;
        private RectTransform _rect;

        public override void LoadDefaults()
        {
            if (!_tmpro || !_rect) return;

            foreach (var lang in _languageData)
            {
                lang.fontMaterial = _tmpro.fontSharedMaterial;
                lang.fontSize = _tmpro.fontSize;
                lang.charSpace = _tmpro.characterSpacing;
                lang.lineSpace = _tmpro.lineSpacing;
                lang.bold = (int)(_tmpro.fontStyle & FontStyles.Bold) > 0;

                lang.pos = _rect.localPosition;
                lang.size = _rect.sizeDelta;
            }
        }



        public override void Init()
        {
            _tmpro = this.GetComponent<TMP_Text>();
            _rect = this.GetComponent<RectTransform>();
        }

        public override void UpdateComponent(LanguageIdentifier languageIdentifier)
        {
            if (!_tmpro || !_rect) return;
            LanguageProperties property = _languageData.Find(x => x.languageKey == I2.Loc.LocalizationManager.CurrentLanguage);

            if (property == null) return;

            _tmpro.fontSize = property.fontSize;
            _tmpro.characterSpacing = property.charSpace;
            _tmpro.lineSpacing = property.lineSpace;

            _tmpro.fontStyle = property.bold ? _tmpro.fontStyle | FontStyles.Bold : _tmpro.fontStyle & ~FontStyles.Bold;

            _rect.localPosition = property.pos;
            _rect.sizeDelta = property.size;
        }

        public override bool IsChanged()
        {
            if (!_tmpro || !_rect) return false;
            LanguageProperties property = _languageData.Find(x => x.languageKey == I2.Loc.LocalizationManager.CurrentLanguage);

            if (property == null) return false;

            bool font = _tmpro.fontSize != property.fontSize;
            bool space = _tmpro.characterSpacing != property.charSpace;
            bool line = _tmpro.lineSpacing != property.lineSpace;

            TMPro.FontStyles style = property.bold ? FontStyles.Bold : ~FontStyles.Bold;
            bool fStyle = _tmpro.fontStyle != style;

            bool pos = (Vector2)_rect.localPosition != property.pos;
            bool rect = _rect.sizeDelta != property.size;

            return font || space || fStyle || line || pos || rect;
        }

        public override void LoadDefaultToActiveLanguage(string languageCode)
        {
            base.ThrowDisabledExertion();

            if (!_tmpro || !_rect) return;

            foreach (var lang in _languageData)
            {
                if (lang.languageKey == languageCode)
                {
                    lang.fontMaterial = _tmpro.fontSharedMaterial;
                    lang.fontSize = _tmpro.fontSize;
                    lang.charSpace = _tmpro.characterSpacing;
                    lang.lineSpace = _tmpro.lineSpacing;
                    lang.bold = (int)(_tmpro.fontStyle & FontStyles.Bold) > 0;

                    lang.pos = _rect.localPosition;
                    lang.size = _rect.sizeDelta;
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
                    _languageData.Add(new LanguageProperties()
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
                    _languageData.Add(new LanguageProperties()
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
