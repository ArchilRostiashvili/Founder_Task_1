using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BebiLibs
{
    public class SettingsToggle : MonoBehaviour
    {
        public GameObject GO_CheckMark;
        public Image Image_ToggleIcon;
        public Image Image_ToggleIconBack;
        public Sprite ToggleActive;
        public Sprite ToggleDisabled;
        public Sprite ToggleActiveBack;
        public Sprite ToggleDisabledBack;
        public Color activeBackColor;
        public Color disabledBackColor;

        public void SetToggleActive(bool value)
        {
            this.GO_CheckMark.SetActive(value);
            this.Image_ToggleIcon.sprite = value ? this.ToggleActive : this.ToggleDisabled;
            this.Image_ToggleIconBack.sprite = value ? this.ToggleActiveBack : this.ToggleDisabledBack;
            this.Image_ToggleIconBack.color = value ? this.activeBackColor : this.disabledBackColor;
        }
    }
}
