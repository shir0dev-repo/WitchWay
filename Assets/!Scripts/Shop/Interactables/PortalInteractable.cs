using UnityEngine;

public class PortalInteractable : MonoBehaviour, IInteractable
{
    public bool Interact(ShopPlayerController player)
    {
        return ShopManager.Instance.LoadArea(ShopArea.Portal);
    }
}
