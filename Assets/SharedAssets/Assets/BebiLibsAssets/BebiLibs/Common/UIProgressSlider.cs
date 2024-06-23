using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIProgressSlider : MonoBehaviour
{
    [SerializeField] private RectTransform _progressFillArea;
    [SerializeField] private RectTransform _progressFill;

    [Range(0, 1)]
    [SerializeField] private float _progress = 0;

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            UpdateProgress(_progress);
        }
    }

    public void UpdateProgress(float value)
    {
        value = Mathf.Clamp01(value);
        float progressBarWidth = _progressFillArea.rect.width;
        _progressFill.sizeDelta = new Vector2(value * progressBarWidth, _progressFill.sizeDelta.y);
    }

}
