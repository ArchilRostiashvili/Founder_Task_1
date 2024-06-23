using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace BebiLibs.ExtendedFreeTrialSystem
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TMP_Text))]
    public class PromotionTextFormater : MonoBehaviour
    {
        [SerializeField] private string _textKey;
        [SerializeField] private PromotionConfigData _promotionConfigData;
        [SerializeField] private TextFormatContainer _textFormatContainer;
        [SerializeField] private TMP_Text _textComponent;
        [SerializeField] private UnityEvent _onTextUpdate;

        private void OnEnable()
        {
            if (_textComponent == null)
            {
                _textComponent = GetComponent<TMP_Text>();
            }

            if (GetUnformattedText(out string unformattedText))
            {
                _textComponent.text = _textFormatContainer.ReplaceString(unformattedText);
            }
            else
            {
                Debug.Log($"Unable to load {_textKey} from config");
            }

            _onTextUpdate?.Invoke();
        }

        //<color=#56FF00>
        //</color>

#if UNITY_EDITOR
        private void Update()
        {
            OnEnable();
        }
#endif

        private bool GetUnformattedText(out string unformattedText)
        {
            PromotionPopUpText promotionPopUpText = _promotionConfigData.GetPromotionPopUpText();
            return promotionPopUpText.TryGetTextFromKey(_textKey, out unformattedText);
        }



    }
}
