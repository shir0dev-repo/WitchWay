using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject DialogueParent;
    public TextMeshProUGUI DialogTitleText;
    public TextMeshProUGUI DialogBodyText;
    public GameObject responseButtonPrefab;
    public Transform responseButtonContainer;

    private DialogueNode currentNode;
    private string currentTitle;
    private int currentLineIndex;

    [Header("Input Actions")]
    public InputAction interactAction;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //HideDialogue();
    }
    
    private void OnEnable()
    {
    interactAction.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
    interactAction.performed -= OnInteractPerformed;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (IsDialogueActive() && currentNode != null)
        {
            ShowNextLine();
        }
    }

    public void StartDialogue(string title, DialogueNode node)
    {
        WZPlayerManager.Instance.SetCanMove(false);
        WZPlayerManager.Instance.SetCursor(true);

        foreach (Transform child in responseButtonContainer)
        {
            Destroy(child.gameObject);
        }
        ShowDialogue();

        currentTitle = title;
        currentNode = node;
        currentLineIndex = 0;

        DialogTitleText.text = title;
        currentNode.onNodeEnter?.Invoke();

        interactAction.Enable();

        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (currentNode == null || currentLineIndex >= currentNode.dialogueLines.Count)
        {
            if (currentNode != null && currentNode.HasResponses())
            {
                ShowResponses();
            }
            else
            {
                HideDialogue();
            }
            return;
        }

        DialogBodyText.text = currentNode.dialogueLines[currentLineIndex];
        currentLineIndex++;

        if (currentLineIndex >= currentNode.dialogueLines.Count && currentNode.HasResponses())
        {
            ShowResponses();
        }
        else if (currentLineIndex > currentNode.dialogueLines.Count)
        {
            HideDialogue();
        }
    }

    private void ShowResponses()
    {
        foreach (Transform child in responseButtonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (DialogueResponse response in currentNode.responses)
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;

            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectResponse(response);
            });
        }
    }

    public void SelectResponse(DialogueResponse response)
    {
        if (response.nextNode != null)
        {
            foreach (Transform child in responseButtonContainer)
        {
            Destroy(child.gameObject);
        }
            StartDialogue(currentTitle, response.nextNode);
            
        }
        else
        {
            HideDialogue();
        }
    }

    public void HideDialogue()
    {
        WZPlayerManager.Instance.SetCanMove(true);
        WZPlayerManager.Instance.SetCursor(false);
        DialogueParent.SetActive(false);
        interactAction.Disable();
        currentNode = null;
    }

    private void ShowDialogue()
    {
        DialogueParent.SetActive(true);
    }

    public bool IsDialogueActive()
    {
        return DialogueParent.activeSelf;
    }
}
