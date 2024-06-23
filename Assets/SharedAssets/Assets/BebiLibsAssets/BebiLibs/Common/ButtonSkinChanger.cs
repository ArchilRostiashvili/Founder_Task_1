using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ButtonSkinChanger : MonoBehaviour
    {
        [Header("Button Components: ")]
        [SerializeField] private Image _buttonImage;
        [SerializeField] private TMP_Text _buttonText;

        [Header("Sprites: ")]
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _disabledSprite;

        [Header("Materials: ")]
        [SerializeField] private Material _defaultTextMaterial;
        [SerializeField] private Material _disabledTextMaterial;

        public void SetActiveState(bool value)
        {
            SetButtonImageSpriteFromState(value);
            SetButtonTextMateriaFromState(value);
        }

        private void SetButtonImageSpriteFromState(bool value)
        {
            if (_buttonImage == null)
            {
                Debug.LogWarning("ButtonSkinChanger: Button image is null!");
                return;
            }

            _buttonImage.sprite = value ? _defaultSprite : _disabledSprite;
        }

        public void SetButtonTextMateriaFromState(bool value)
        {
            if (_buttonText == null)
            {
                Debug.LogWarning("ButtonSkinChanger: Button text is null!");
                return;
            }

            _buttonText.fontMaterial = value ? _defaultTextMaterial : _disabledTextMaterial;
        }
    }
}
