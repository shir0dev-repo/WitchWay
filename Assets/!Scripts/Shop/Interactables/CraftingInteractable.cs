using UnityEngine;
using UnityEngine.SceneManagement;

public class CraftingInteractable : MonoBehaviour, IInteractable
{
    public bool Interact(ShopPlayerController player)
    {
        SceneManager.LoadScene("Crafting", LoadSceneMode.Additive);
        return true;
    }
}
