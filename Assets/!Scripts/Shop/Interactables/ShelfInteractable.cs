using UnityEngine;

public class ShelfInteractable : MonoBehaviour, IInteractable
{
    public bool Interact(ShopPlayerController player)
    {
        return ShopManager.Instance.LoadArea(ShopArea.Shelves);
    }
}
