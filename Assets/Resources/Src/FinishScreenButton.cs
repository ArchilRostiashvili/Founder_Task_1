
using System;
using BebiLibs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishScreenButton : MonoBehaviour
{
    public Action<FarmMinigameSceneData> ButtonClickedEvent;

    [SerializeField] private Image _thumbnailImage;
    [SerializeField] private ButtonScale _button;
    private FarmMinigameSceneData _minigameData;

    public void SetData(FarmMinigameSceneData data)
    {
        _thumbnailImage.sprite = data.Thumbnail;
        _minigameData = data;
    }

    public void LoadLevel()
    {
        ButtonClickedEvent?.Invoke(_minigameData);
    }

    internal void DisableButton()
    {
        _button.enabled = false;
    }
}
