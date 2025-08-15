using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject   dialogueParent;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private GameObject   responseButtonPrefab;
    [SerializeField] private Transform    responseButtonContainer;

    [Header("Input Actions")]
    [SerializeField] private InputAction  interactAction;

    private DialogueNode currentNode;
    private bool isTyping;
    private Coroutine typingCo;
    private bool justStartedDialogue = false;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void OnEnable()  => interactAction.performed += OnInteract;
    private void OnDisable() => interactAction.performed -= OnInteract;

    public void StartDialogue(DialogueNode node)
    {
        if (node == null) return;

        if (WZPlayerController.Instance != null)
        {
            WZPlayerManager.Instance.ToggleInput(false);
            WZPlayerManager.Instance.ToggleCursor(true);
        }
        else if (ShopPlayerController.Instance != null)
        {
            ShopPlayerController.Instance.ToggleInput(ShopPlayerController.InputMode.Locked);
        }

        ClearResponses();
        dialogueParent.SetActive(true);

        currentNode = node;

        justStartedDialogue = true;
        StartCoroutine(ClearJustStartedFlagNextFrame());

        ShowLine(node);

        interactAction.Enable();
    }

    private IEnumerator ClearJustStartedFlagNextFrame()
    {
        yield return null;
        justStartedDialogue = false;
    }

    private void ShowLine(DialogueNode node)
    {
        if (typingCo != null) StopCoroutine(typingCo);

        titleText.text = node.speakerName;

        bodyText.text = string.Empty;
        isTyping = true;

        node.onNodeEnter?.Invoke();

        typingCo = StartCoroutine(TypeLine(node.line, node.typingSpeed));
    }

    private IEnumerator TypeLine(string line, float speed)
    {
        foreach (char c in line)
        {
            bodyText.text += c;
            yield return new WaitForSeconds(speed);
        }
        isTyping = false;

        if (currentNode.responses.Count > 0)
            ShowResponses();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
{
    if (!IsDialogueActive() || currentNode == null || justStartedDialogue) return;

    if (isTyping)
    {
        if (typingCo != null) StopCoroutine(typingCo);
        bodyText.text = currentNode.line;
        isTyping = false;

        if (currentNode.responses.Count > 0)
            ShowResponses();

        return;
    }

    if (responseButtonContainer.childCount > 0) return;

    if (currentNode.nextNode != null)
    {
        StartDialogue(currentNode.nextNode);
    }
    else
    {
        HideDialogue();
    }
}


    private void ShowResponses()
    {
        ClearResponses();

        foreach (DialogueResponse resp in currentNode.responses)
        {
            GameObject btnObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = resp.responseText;
            btnObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(resp));
        }
    }

    public void SelectResponse(DialogueResponse resp)
    {
        ClearResponses();
        if (resp.nextNode != null)
            StartDialogue(resp.nextNode);
        else
            HideDialogue();
    }

    private void ClearResponses()
    {
        foreach (Transform child in responseButtonContainer)
            Destroy(child.gameObject);
    }

    public void HideDialogue()
    {
        if (typingCo != null) StopCoroutine(typingCo);

        dialogueParent.SetActive(false);
        interactAction.Disable();
        currentNode = null;
        if (WZPlayerManager.Instance != null)
        {
            WZPlayerManager.Instance.ToggleInput(true);
            WZPlayerManager.Instance.ToggleCursor(false);
        }
        else if (ShopPlayerController.Instance != null)
        {
            ShopPlayerController.Instance.ToggleInput(ShopPlayerController.InputMode.Freed);
        }
    }

    public bool IsDialogueActive() => dialogueParent.activeSelf;
}
