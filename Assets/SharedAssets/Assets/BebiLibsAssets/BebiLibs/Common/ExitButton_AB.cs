using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BebiLibs;
public class ExitButton_AB : MonoBehaviour
{
    public GameObject GO_buttonTap, GO_buttonSlide;
    public TextMeshProUGUI TMP_text;
    public Toggle T_toggle;
    private bool _isDrag = true;
    public void Start()
    {
        if (this.GO_buttonTap.GetComponent<ButtonScale>())
        {
            this.GO_buttonTap.GetComponent<ButtonScale>().Init();

        }
    }
    public void ChangeButtons(bool state)
    {
        this.GO_buttonSlide.SetActive(state);
        this.GO_buttonTap.SetActive(!state);
    }
    public void ToggleExitButton()
    {
        if (T_toggle.isOn)
        {
            _isDrag = true;
            this.TMP_text.text = "Drag button";
        }
        else
        {
            _isDrag = false;
            this.TMP_text.text = "Tap button";
        }
        this.ChangeButtons(_isDrag);
    }

    public bool GetIsDrag()
    {
        return _isDrag;
    }
}
