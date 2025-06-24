using UnityEngine;
using System.Collections.Generic;

public class DialogueFunctions : MonoBehaviour
{
    [Header("Nodes that decrease sanity when entered")]
    public List<DialogueNode> sanityDownNodes = new List<DialogueNode>();
    [Header("Nodes that increase sanity when entered")]
    public List<DialogueNode> sanityUpNodes = new List<DialogueNode>();

    void Awake()
    {
        foreach (var node in sanityDownNodes)
        {
            //node.onNodeEnter.AddListener(() => DecreasePlayerSanity());
        }
        foreach (var node in sanityUpNodes)
        {
            //node.onNodeEnter.AddListener(() => IncreasePlayerSanity());
        }
    }

    public static void DecreasePlayerSanity()
    {
        if (WZPlayerManager.Instance != null)
        {
            WZPlayerManager.Instance.DecreaseSanity(1);
        }
    }

    public static void IncreasePlayerSanity()
    {
        if (WZPlayerManager.Instance != null)
        {
            WZPlayerManager.Instance.IncreaseSanity(1);
        }
    }
}