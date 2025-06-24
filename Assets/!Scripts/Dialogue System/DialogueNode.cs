using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Dialogue Node", menuName = "Dialogue/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    public List<string> dialogueLines = new List<string>();
    public List<DialogueResponse> responses = new List<DialogueResponse>();
    public UnityEvent onNodeEnter;
    public string CharacterName;

    public bool HasResponses()
    {
        return responses != null && responses.Count > 0;
    }
}
