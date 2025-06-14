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

    //private vars
    Camera cam;

    private RaycastHit lastHit;
    private bool didHit;

    private Color baseReticleColor;
    private Vector2 baseReticleSize;

    private bool paused = false;

    void Awake()
    {
        cam = Camera.main;

        //store reticle values
        baseReticleColor = reticleImage.color;
        baseReticleSize = reticleImage.rectTransform.sizeDelta;
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

        optionChangeAction.started += OnDiaOptionChange;
        selectAction.started += OnDiaOptionSelect;

        showIngrediantsAction.performed += OnShowIngredients;
        recipeBookAction.performed += OnShowRecipes;
        pauseAction.started += OnPauseGame;
    }

    void OnDisable()
    {
        interactAction.started -= OnInteract;
        dragAction.performed -= OnDragObject;

        optionChangeAction.started -= OnDiaOptionChange;
        selectAction.started -= OnDiaOptionSelect;

        showIngrediantsAction.performed -= OnShowIngredients;
        recipeBookAction.performed -= OnShowRecipes;
        pauseAction.started -= OnPauseGame;

        interactAction.Disable();
        dragAction.Disable();
        optionChangeAction.Disable();
        selectAction.Disable();
        showIngrediantsAction.Disable();
        recipeBookAction.Disable();
        pauseAction.Disable();
    }

    void Update()
    {
        CastInteractRay(ingredientObjectTag, draggableObjectTag, npcObjectTag);
    }

    //interaction controls
    private void OnInteract(InputAction.CallbackContext context)
    {
        GameObject interactedObject = CheckForInteractable(ingredientObjectTag, npcObjectTag); //if null no object found
        if (interactedObject != null)
        {
            if (interactedObject.CompareTag(ingredientObjectTag))
            {
                IngrediantInteracted();
            }
            else //only not null if one of the two passed in so no need to check for both
            {
                NPCInteracted();
            }
        }
    }

    //interacted with an ingrediant
    private void IngrediantInteracted()
    {
        //add ingrediant to inventory
        //inventory system needs to beredone cause idk why i didnt think of this
        print("ingredient");
    }

    //interacted with an npc
    private void NPCInteracted()
    {
        //disable other input
        WZPlayerController playerController = GetComponent<WZPlayerController>();
        playerController.enabled = false;

        //unlock cursor and hide reticle
        Cursor.lockState = CursorLockMode.Confined;
        reticleImage.gameObject.SetActive(false);

        print("NPC interacted");

        //NEEDS TO BE REENABLED SOMEWHERE AT SOME POINT
    }

    private void OnDragObject(InputAction.CallbackContext context)
    {

    }

    //dialogue controls
    private void OnDiaOptionChange(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
    }

    private void OnDiaOptionSelect(InputAction.CallbackContext context)
    {

    }

    //misc controls
    private void OnShowIngredients(InputAction.CallbackContext context)
    {

    }

    private void OnShowRecipes(InputAction.CallbackContext context)
    {

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
