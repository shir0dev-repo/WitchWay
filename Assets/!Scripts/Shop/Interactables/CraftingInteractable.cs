using UnityEngine;
using UnityEngine.SceneManagement;

public class CraftingInteractable : MonoBehaviour, IInteractable
{
    public bool Interact(ShopPlayerController player)
    {
        FindFirstObjectByType<ShopManager>().LoadArea(ShopArea.CraftingStation);
        return true;
    }
}
