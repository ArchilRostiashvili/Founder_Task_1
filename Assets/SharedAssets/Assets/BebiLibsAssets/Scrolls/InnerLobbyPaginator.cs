using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InnerLobbyPaginator : CarouselScrollBehaviour
{
    [SerializeField] private List<Image> _paginationCircleList = new List<Image>();
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Sprite _deselectedSprite;
    public Transform paginatorParent;
    public GameObject prefab_spawnElement;
    public bool isInitialized;

    public void UpdateElements(int index)
    {
        for (int i = 0; i < _paginationCircleList.Count; i++)
        {
            if (i == index)
            {
                _paginationCircleList[i].sprite = _selectedSprite;
            }
            else
            {
                _paginationCircleList[i].sprite = _deselectedSprite;
            }
        }
    }


    public override void Initialize(CarouselScroll scroll)
    {
        if (isInitialized) return;
        foreach (Transform child in paginatorParent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public override void OnFinishScrollUpdate(CarouselScroll scroll)
    {
        if (isInitialized) return;
        isInitialized = true;

        _paginationCircleList.Clear();

        for (int i = 0; i < scroll._scrollElementList.Count; i++)
        {
            Image image = GameObject.Instantiate(prefab_spawnElement, paginatorParent).GetComponent<Image>();
            _paginationCircleList.Add(image);
        }

        UpdateElements(0);
    }

    public override void OnScrollChange(ScrollElement newElement)
    {
        UpdateElements(newElement.PaginationID);
    }

    public override void UpdateScroll(CarouselScroll scroll)
    {

    }

    public override void Clear()
    {

    }


}
