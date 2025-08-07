using UnityEngine;

public class DialogueActor : MonoBehaviour
{
    [SerializeField]
    private DialogueNode rootNode;

    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(rootNode);
    }
}