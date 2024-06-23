using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollElement : MonoBehaviour
{
    [SerializeField] private RectTransform _elementRT;
    [SerializeField] private RectTransform _contentRT;

    [SerializeField] private RectTransform _spaceDividerRT;
    [SerializeField] private bool _canCenterElement = false;

    public Vector2 ScreenSize { get; set; }
    public float LeftSpawnPosition { get; set; }
    public float RightSpawnPosition { get; set; }
    public Vector3 DefaultPosition { get; set; }

    public Vector2 WorldSize { get; set; }
    public Vector2 PanelLeftSize { get; set; }
    public int PaginationID { get; set; }

    public bool CanCenterElement => _canCenterElement;

    public RectTransform ElementRT => _elementRT;


    [System.Obsolete()]
    public string scrollHeaderText => _scrollHeader;
    [System.Obsolete()]
    [SerializeField] private string _scrollHeader;
    [System.Obsolete()]
    [SerializeField] private Sprite _categoryIcon;
    [System.Obsolete()]
    public Sprite categoryIcon => _categoryIcon;

    public virtual void Init()
    {
        if (_elementRT == null)
            _elementRT = GetComponent<RectTransform>();
    }

    public void UpdatePositions()
    {
        Init();
        ScreenSize = _elementRT.sizeDelta;
        WorldSize = Utils.GetWorldRect(_elementRT).size;
        if (_spaceDividerRT != null)
        {
            PanelLeftSize = Utils.GetWorldRect(_spaceDividerRT).size;
        }
        DefaultPosition = position;
    }

    public Vector3 position
    {
        get => _elementRT.transform.position + (new Vector3(PanelLeftSize.x, 0, 0) / 2);
        set => _elementRT.transform.position = value - (new Vector3(PanelLeftSize.x, 0, 0) / 2);
    }

    public Vector3 anchoredPosition
    {
        get => _elementRT.anchoredPosition;
        set => _elementRT.anchoredPosition = value;
    }
}
