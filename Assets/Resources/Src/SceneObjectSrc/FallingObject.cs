using UnityEngine;
using UnityEngine.Events;

public class FallingObject : MonoBehaviour
{
    public bool IsAlreadyFallen => _isAlreadyFallen;

    [SerializeField] private UnityEvent OnInteract;
    
    private bool _isAlreadyFallen;

    public void Interact()
    {
        _isAlreadyFallen = true;
        OnInteract?.Invoke();
    }

    public void ResetFallingObject() => _isAlreadyFallen = false;
}
