using UnityEngine;
using UnityEngine.Events;

public class WZInteractable : MonoBehaviour
{
    public UnityEvent onInteract;

    public void Interacted()
    {
        onInteract?.Invoke();
    }
}
