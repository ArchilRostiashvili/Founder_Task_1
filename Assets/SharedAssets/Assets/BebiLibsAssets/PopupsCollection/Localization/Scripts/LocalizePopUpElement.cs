using BebiLibs.Localization.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BebiLibs
{
    public class LocalizePopUpElement : MonoBehaviour
    {
        public Image Image_Icon;
        public LanguageIdentifier localLanguageType;
        public GameObject checkMarkImage;

        public void SetCheckMarkActive(bool value)
        {
            this.checkMarkImage.SetActive(value);
        }

        public void SetImage(Sprite sprite)
        {
            this.Image_Icon.sprite = sprite;
        }

    }
}
