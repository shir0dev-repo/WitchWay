using UnityEngine;
using UnityEngine.SceneManagement;

public class CraftingInteractable : MonoBehaviour, IInteractable
{
    public bool Interact(ShopPlayerController player)
    {
        return ShopManager.Instance.LoadArea(ShopArea.CraftingStation);
    }
}
