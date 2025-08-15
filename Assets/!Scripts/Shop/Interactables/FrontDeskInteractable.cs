using UnityEngine;

public class FrontDeskInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueActor _dialogueActor;

    public bool Interact(ShopPlayerController player)
    {
        _dialogueActor.Interact();
        return true;
    }
}
