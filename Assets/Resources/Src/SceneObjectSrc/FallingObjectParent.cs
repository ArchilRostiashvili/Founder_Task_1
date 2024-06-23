using System.Collections.Generic;
using UnityEngine;

public class FallingObjectParent : MonoBehaviour
{
    [SerializeField] private List<FallingObject> _fallingObjectsList;

    public void InteractRandomObject()
    {
        FallingObject fallingObject = _fallingObjectsList.Find(x => !x.IsAlreadyFallen);
        
        if (fallingObject != null)
        {
            fallingObject.Interact();
        }
    }
}
