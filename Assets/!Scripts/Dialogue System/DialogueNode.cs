using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Dialogue Node", menuName = "Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    public string nodeID;
    public string speakerName;
    [TextArea] public string line;
    public float typingSpeed = 0.05f;

    public string nextNodeID;
    public DialogueNode nextNode;
    public List<DialogueResponse> responses = new();

    public UnityEvent onNodeEnter;
}

