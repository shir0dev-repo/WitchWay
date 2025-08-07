using UnityEngine;
using System.Collections.Generic;

public class WZDialogueMonsterFunctions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WZDialogueMonsterAI dialogueMonsterAI;

    [Header("Nodes that change sanity when entered")]
    [SerializeField] private List<SanityChangeEntry> sanityChangeEntries = new List<SanityChangeEntry>();

    [Header("Node that completes dialogue")]
    [SerializeField] private DialogueNode[] dialogueCompleteNode;

    [System.Serializable]
    public class SanityChangeEntry
    {
        public DialogueNode node;
        public int sanityChange;
    }

    private Dictionary<DialogueNode, int> changeSanityNodes = new Dictionary<DialogueNode, int>();

    void Awake()
    {
        foreach (var entry in sanityChangeEntries)
        {
            if (entry.node != null && !changeSanityNodes.ContainsKey(entry.node))
            {
                changeSanityNodes.Add(entry.node, entry.sanityChange);
                entry.node.onNodeEnter.AddListener(() => ChangePlayerSanity(entry.sanityChange));
            }
        }
        foreach (var node in dialogueCompleteNode)
        {
            node.onNodeEnter.AddListener(OnDialogueCompleted);
        }
    }

    public static void ChangePlayerSanity(int change)
    {
        if (WZPlayerManager.Instance != null)
        {
            WZPlayerManager.Instance.ModifySanity(change);
        }
    }

    public void OnDialogueCompleted()
    {
        if (dialogueMonsterAI != null)
        {
            dialogueMonsterAI.CompleteDialogue();
        }
    }
}
