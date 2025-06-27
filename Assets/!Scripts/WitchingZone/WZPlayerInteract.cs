using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WZPlayerInteract : MonoBehaviour
{
    [Header("Interact Controls")]
    [SerializeField] private InputAction interactAction;
    [Space(5)]
    [SerializeField] private InputAction dragAction;

    [Header("Dialogue Controls")]
    [SerializeField] private InputAction optionChangeAction; //wasd and arrow keys
    [Space(5)]
    [SerializeField] private InputAction selectAction;

    [Header("Misc Controls")]
    [SerializeField] private InputAction showIngrediantsAction;
    [Space(5)]
    [SerializeField] private InputAction recipeBookAction;
    [Space(5)]
    [SerializeField] private InputAction pauseAction;

    [Header("Interact Settings")]
    [SerializeField] private Image reticleImage;
    [SerializeField] private string ingredientObjectTag; //these can be changed if/when they have unique scripts
    [SerializeField] private string draggableObjectTag;
    [SerializeField] private string npcObjectTag;
    [SerializeField] private float pickupDistance;
    [SerializeField] private float objectDragSpeed = 20f;

    [Header("UI Objects")]
    [SerializeField] private CanvasGroup inventoryCanvasGroup;

    //private vars
    Camera cam;

    private RaycastHit lastHit;
    private bool didHit;

    private Color baseReticleColor;
    private Vector2 baseReticleSize;

    private bool paused = false;

    private GameObject currentlyDragging;
    private Vector3 hitPosition;

    void Awake()
    {
        cam = Camera.main;

        //store reticle values
        baseReticleColor = reticleImage.color;
        baseReticleSize = reticleImage.rectTransform.sizeDelta;

        inventoryCanvasGroup.alpha = 0;
    }

    void OnEnable()
    {
        interactAction.Enable();
        dragAction.Enable();
        optionChangeAction.Enable();
        selectAction.Enable();
        showIngrediantsAction.Enable();
        recipeBookAction.Enable();
        pauseAction.Enable();

        interactAction.started += OnInteract;
        dragAction.performed += OnDragObject;
        dragAction.canceled += OffDragObject;

        optionChangeAction.started += OnDiaOptionChange;
        selectAction.started += OnDiaOptionSelect;

        showIngrediantsAction.performed += OnShowIngredients;
        showIngrediantsAction.canceled += UnShowIngredients;
        recipeBookAction.performed += OnShowRecipes;
        recipeBookAction.canceled += UnShowRecipes;
        pauseAction.started += OnPauseGame;
    }

    void OnDisable()
    {
        interactAction.started -= OnInteract;
        dragAction.performed -= OnDragObject;
        dragAction.canceled -= OffDragObject;

        optionChangeAction.started -= OnDiaOptionChange;
        selectAction.started -= OnDiaOptionSelect;

        showIngrediantsAction.performed -= OnShowIngredients;
        showIngrediantsAction.canceled -= UnShowIngredients;
        recipeBookAction.performed -= OnShowRecipes;
        recipeBookAction.canceled -= UnShowRecipes;
        pauseAction.started -= OnPauseGame;

        interactAction.Disable();
        dragAction.Disable();
        optionChangeAction.Disable();
        selectAction.Disable();
        showIngrediantsAction.Disable();
        recipeBookAction.Disable();
        pauseAction.Disable();
    }

    public void SetControlsEnabled(bool enabled)
    {
        if (enabled)
        {
            interactAction.Enable();
            dragAction.Enable();
            optionChangeAction.Enable();
            selectAction.Enable();
            showIngrediantsAction.Enable();
            recipeBookAction.Enable();
            pauseAction.Enable();
        }
        else
        {
            interactAction.Disable();
            dragAction.Disable();
            optionChangeAction.Disable();
            selectAction.Disable();
            showIngrediantsAction.Disable();
            recipeBookAction.Disable();
            pauseAction.Disable();
        }
    }

    void Update()
    {
        CastInteractRay(ingredientObjectTag, draggableObjectTag, npcObjectTag);

        DragObject();
    }

    //interaction controls
    private void OnInteract(InputAction.CallbackContext context)
    {
        GameObject interactedObject = CheckForInteractable(ingredientObjectTag, npcObjectTag); //if null no object found
        if (interactedObject != null)
        {
            if (interactedObject.CompareTag(ingredientObjectTag))
            {
                IngrediantInteracted(interactedObject.GetComponent<WZWorldIngredient>());
            }
            else if (interactedObject.CompareTag(npcObjectTag) && !DialogueManager.Instance.IsDialogueActive())
            {
                interactedObject.GetComponent<DialogueActor>()?.Interact();
            }
            else
            {
                Debug.LogWarning("Interacted with an object that is not an ingredient or NPC: " + interactedObject.name);
            }
        }
    }

    //interacted with an ingrediant (this adds to ALL ingrediants, including ones you have previously collected. if this isnt desired i can change it)
    private void IngrediantInteracted(WZWorldIngredient ingredient)
    {
        // Save Logic, blame Sara
        var saveable = ingredient.GetComponent<SaveableItem>();
        if (saveable != null && !string.IsNullOrEmpty(saveable.itemID))
        {
            switch (saveable.itemType)
            {
                case SaveItemType.Ingredient:
                    SaveManager.Instance.CollectIngredient(saveable.itemID);
                    break;
                case SaveItemType.Bottle:
                    SaveManager.Instance.CollectBottle(saveable.itemID);
                    break;
            }
        }

        //add ingrediant to inventory
        Inventory inventory = GetComponent<Inventory>();
        inventory.AddNewItem(ingredient.ingredient);

        Destroy(ingredient.gameObject);
    }

    private void OnDragObject(InputAction.CallbackContext context)
    {
        currentlyDragging = CheckForInteractable(draggableObjectTag);
        if (currentlyDragging != null)
        {
            Rigidbody draggedRb = currentlyDragging.GetComponent<Rigidbody>();
            draggedRb.useGravity = false;
            draggedRb.linearVelocity = Vector3.zero;

            hitPosition = lastHit.point;
        }
    }

    private void OffDragObject(InputAction.CallbackContext context)
    {
        if (currentlyDragging != null)
        {
            Rigidbody draggedRb = currentlyDragging.GetComponent<Rigidbody>();
            draggedRb.useGravity = true;
        }

        currentlyDragging = null;
    }

    private void DragObject()
    {
        if (currentlyDragging != null)
        {
            Vector3 targetWorldPosition = cam.transform.position + cam.transform.forward * Vector3.Distance(cam.transform.position, hitPosition);

            Rigidbody draggedRb = currentlyDragging.GetComponent<Rigidbody>();
            Vector3 direction = targetWorldPosition - currentlyDragging.transform.position;

            draggedRb.linearVelocity = direction * objectDragSpeed;
        }
    }

    //dialogue controls
    private void OnDiaOptionChange(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        //blank until dialogue system
    }

    private void OnDiaOptionSelect(InputAction.CallbackContext context)
    {
        //also blank until dialogue system
    }

    //misc controls
    private void OnShowIngredients(InputAction.CallbackContext context)
    {
        inventoryCanvasGroup.alpha = 1;
    }

    private void UnShowIngredients(InputAction.CallbackContext context)
    {
        inventoryCanvasGroup.alpha = 0;
    }

    private void OnShowRecipes(InputAction.CallbackContext context)
    {
        print("showing recipes");
    }

    private void UnShowRecipes(InputAction.CallbackContext context)
    {
        print("stop showing recipes");
    }

    private void OnPauseGame(InputAction.CallbackContext context)
    {
        //this could be swapped for just disabling inputs 
        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //add code for a Ui appearing
    }

    //utility
    private void CastInteractRay(params string[] tagsToCheck)
    {
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = cam.ScreenPointToRay(center);

        didHit = Physics.Raycast(ray, out lastHit, pickupDistance);
        if (didHit)
        {
            foreach (string tag in tagsToCheck)
            {
                if (lastHit.transform.CompareTag(tag))
                {
                    //maybe store these at start?
                    Color newRetColor = new Color(baseReticleColor.r, baseReticleColor.g, baseReticleColor.b, baseReticleColor.a + (baseReticleColor.a * 0.25f));
                    reticleImage.color = newRetColor;

                    Vector2 newRetSize = new Vector2(baseReticleSize.x + (baseReticleSize.x * 0.25f), baseReticleSize.y + (baseReticleSize.y * 0.25f));
                    reticleImage.rectTransform.sizeDelta = newRetSize;

                    break;
                }
                else
                {
                    reticleImage.color = baseReticleColor;
                    reticleImage.rectTransform.sizeDelta = baseReticleSize;
                }
            }
        }
        else
        {
            reticleImage.color = baseReticleColor;
            reticleImage.rectTransform.sizeDelta = baseReticleSize;
        }

#if UNITY_ENGINE
        Debug.DrawRay(ray.origin, ray.direction * pickupDistance, Color.red);
#endif
    }

    //check if the hit ingrediant is a correct type
    private GameObject CheckForInteractable(params string[] tagsToCheck)
    {
        //send out ray from camera center
        if (!didHit) return null;

        foreach (string tag in tagsToCheck)
        {
            if (lastHit.transform.CompareTag(tag))
            {
                return lastHit.transform.gameObject;
            }
        }
        return null;
    }
}
