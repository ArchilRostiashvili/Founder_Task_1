using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollPaginator : MonoBehaviour, IScrollChangeListener
{
    [SerializeField] private GameObject _paginationPrefabGO;

    [SerializeField] private List<Image> _paginationCircleList = new List<Image>();
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Sprite _deselectedSprite;


    public void Initialize(int pageCount, Transform pagerSpawnParent = null)
    {
        Transform spawnParent = pagerSpawnParent == null ? this.transform : pagerSpawnParent;
        InstantiatePagers(pageCount, spawnParent);
        UpdateElements(0);
    }

    private void InstantiatePagers(int pageCount, Transform spawnParent)
    {
        _paginationCircleList.Clear();
        for (int i = 0; i < pageCount; i++)
        {
            GameObject pagElement = GameObject.Instantiate(_paginationPrefabGO, spawnParent);
            _paginationCircleList.Add(pagElement.GetComponent<Image>());
        }
    }

    private void UpdateElements(int index)
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

    public void OnScrollChange(int scrollElementIndex)
    {
        UpdateElements(scrollElementIndex);
    }


    public void Clear()
    {
        for (int i = 0; i < _paginationCircleList.Count; i++)
        {
            if (_paginationCircleList[i] != null)
            {
                GameObject.Destroy(_paginationCircleList[i].gameObject);
            }
        }

        _paginationCircleList.Clear();
    }

}
