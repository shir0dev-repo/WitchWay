using UnityEngine;

public class DialogueActor : MonoBehaviour
{
    [SerializeField]
    private string Name;
    [SerializeField]
    private Dialogue Dialogue;

    [SerializeField]
    private Renderer targetRenderer;
    [SerializeField]
    public Material defaultMaterial;
    [SerializeField]
    public Material highlightMaterial;

    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode);
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         if (targetRenderer != null && highlightMaterial != null)
    //         {
    //             targetRenderer.material = highlightMaterial;
    //         }
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         if (targetRenderer != null && defaultMaterial != null)
    //         {
    //             targetRenderer.material = defaultMaterial;
    //         }
    //     }
    // }
}