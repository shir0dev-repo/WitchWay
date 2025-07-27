using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[System.Serializable]
public class BasketItems
{
    public Transform basket;
    public ModifiedIngredient assignedIngredient;
    public int itemAmount;

    public BasketItems(Transform basket, ModifiedIngredient assignedIngredient, int itemAmount)
    {
        this.basket = basket;
        this.assignedIngredient = assignedIngredient;
        this.itemAmount = itemAmount;
    }
}

[System.Serializable]
public class CraftingRectArea
{
    public RectTransform screenRect;
    public float depthValue;
}

public class StationsInventory : MonoBehaviour
{
    public static StationsInventory Instance { get; private set; }

    [Header("Basket Settings")]
    [SerializeField] private float spacing;

    [Header("Positioning")]
    [SerializeField] private float yPos;
    [SerializeField] private float zPos;

    [Header("Station Canvases")]
    [SerializeField] private CraftingRectArea[] craftingRects;
    public CraftingRectArea[] GetCraftingRects()
    {
        return craftingRects;
    }

    [SerializeField] private int _destroySectionIndex = 0;
    public int DestroySectionIndex => _destroySectionIndex;


    //private vars
    [SerializeField] private StationsInvBox[] boxes;
    private List<IngredientSO> ingredients = new List<IngredientSO>();

    private bool startDelayed = false;

    [Header("Sound")]
    [SerializeField] private EventReference OnItemTrashedSound;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        //CreateBaskets();
        //AddItemsToBaskets();
    }

    private void OnEnable()
    {
        GameEvents.Crafting.OnStationChanged += OnStationChangedHandler;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnStationChanged -= OnStationChangedHandler;
    }

    void Start()
    {
        //setup triggers
        GameEvents.Crafting.OnItemPlacedInTrash += PermanentRemove;

        PopulateIngredients();
        SortIntoBoxes();

        StartCoroutine(WaitAfterStart());
    }

    private void PopulateIngredients()
    {
        foreach (InventorySlot slot in PersistantItemList.inventorySlots)
        {
            for (int i = 0; i < slot.ingredientAmt; i++)
                ingredients.Add(slot.ingredient);
        }
    }

    private void SortIntoBoxes()
    {
        foreach (StationsInvBox box in boxes)
        {
            box.ClearItems();
        }

        //quantify 
        List<BasketItems> items = new();
        foreach (IngredientSO ingred in ingredients)
        {
                ModifiedIngredient newMIngred = new ModifiedIngredient();
                newMIngred.BaseIngredient = ingred;
                items.Add(new BasketItems(null, newMIngred, 1));
        }

        //prep data
        int[] boxCounts = new int[boxes.Length];
        int currentBox = 0;

        foreach (BasketItems item in items)
        {
            int remaining = item.itemAmount;

            while (remaining > 0)
            {
                int tries = 0;
                while (boxCounts[currentBox] >= 15 && tries < boxes.Length)
                {
                    currentBox = (currentBox + 1) % boxes.Length;
                    tries++;
                }

                if (tries >= boxes.Length)
                {
                    // NOTE: Should never happen ! Check if player adds too many ingredients from the shelf, before reaching this
                    Debug.LogWarning("too many");
                    return;
                }

                boxes[currentBox].AddItem(new BasketItems(boxes[currentBox].transform, item.assignedIngredient, 1));

                boxCounts[currentBox]++;
                remaining--;

                currentBox = (currentBox + 1) % boxes.Length;
            }
        }
    }

    public void PermanentRemove(WorldIngredient ingredient)
    {
        for (int i = 0; i < PersistantItemList.inventorySlots.Count; i++)
        {
            InventorySlot slot = PersistantItemList.inventorySlots[i];
            if (slot.ingredient == ingredient.BaseIngredient)
            {
                if (slot.ingredientAmt > 1)
                {
                    slot.ingredientAmt -= 1;
                }
                else
                {
                    PersistantItemList.inventorySlots.RemoveAt(i);
                    SoundManager.Instance.PlayOneShot(OnItemTrashedSound, ingredient.transform.position);
                }

                break;
            }
        }
    }

    private void OnStationChangedHandler(int stationId)
    {
        print("station changed");
        if (startDelayed)
        {
            StartCoroutine(SendToInventoryCoroutine());
        }
    }

    private IEnumerator WaitAfterStart()
    {
        yield return new WaitForSeconds(0.5f);

        startDelayed = true;
    }

    private IEnumerator SendToInventoryCoroutine()
    {
        WorldIngredient[] ingredientsInWorld = FindObjectsByType<WorldIngredient>(FindObjectsSortMode.None);
        Queue<WorldIngredient> ingredientsToReturn = new Queue<WorldIngredient>(ingredientsInWorld);

        int boxIndex = 0;
        StationsInvBox invBox = boxes[boxIndex];
        while (ingredientsToReturn.TryDequeue(out WorldIngredient wIng))
        {
            if (wIng.TryGetComponent(out HoverToLocation hover))
            {
                hover.Target = invBox.transform;
                boxIndex = (boxIndex + 1) % boxes.Length;
            }

            yield return null;
        }
    }
}
