using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectRandomRow : MonoBehaviour
{
    [SerializeField] private bool _isRandomObject = true;

    [SerializeField] private List<FallingObjectParent> _fallingObjectsParentList;
    [SerializeField][HideField("_isRandomObject", false)] private FallingObjectParent _activeFallingObject;

    public int FallingObjectsCount() => _fallingObjectsParentList.Count;

    public void SpawnRandomObject(int index)
    {
        if (!_isRandomObject)
            return;

        _fallingObjectsParentList[index].gameObject.SetActive(true);
        _activeFallingObject = _fallingObjectsParentList[index];
    }

    public void InteractRandomObject()
    {
        _activeFallingObject.InteractRandomObject();
    }
}
