using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasketItems
{
    public Transform basket;
    public IngredientSO assignedIngredient;
    public int itemAmount;

    public BasketItems(Transform basket, IngredientSO assignedIngredient, int itemAmount)
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
    [SerializeField] private GameObject[] boxes;
    private List<IngredientSO> ingredients = new List<IngredientSO>();

    private StationManager stationManger;

    private bool startDelayed = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        //CreateBaskets();
        //AddItemsToBaskets();
    }

    void Start()
    {
        stationManger = StationManager.Instance;
        if (stationManger != null) stationManger.OnStationChanged.AddListener(OnStationChangedHandler);

        //setup triggers
        GameEvents.Crafting.OnItemPlacedInTrash += PermanentRemove;

        PopulateIngredients();
        SortIntoBoxes();

        StartCoroutine(WaitAfterStart());
    }

    void Update()
    {

    }

    private void PopulateIngredients()
    {
        print(PersistantItemList.inventorySlots.Count);
        foreach (InventorySlot slot in PersistantItemList.inventorySlots)
        {
            ingredients.Add(slot.ingredient);
        }
    }

    private void SortIntoBoxes()
    {
        foreach (GameObject box in boxes)
        {
            box.GetComponent<StationsInvBox>().ClearItems();
        }

        //quantify 
        Dictionary<string, BasketItems> groupedItems = new Dictionary<string, BasketItems>();
        foreach (IngredientSO ingred in ingredients)
        {
            if (groupedItems.ContainsKey(ingred.name))
            {
                groupedItems[ingred.name].itemAmount += 1;
            }
            else
            {
                groupedItems[ingred.name] = new BasketItems(null, ingred, 1);
            }
        }

        //prep data
        int[] boxCounts = new int[boxes.Length];
        int currentBox = 0;

        foreach (BasketItems bValue in groupedItems.Values)
        {
            int remaining = bValue.itemAmount;

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
                    Debug.LogWarning("too many");
                    return;
                }

                TestInvBox boxScript = boxes[currentBox].GetComponent<TestInvBox>();
                boxScript.AddItem(new BasketItems(boxes[currentBox].transform, bValue.assignedIngredient, 1));

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
                }

                break;
            }
        }
    }

    private void OnStationChangedHandler(int stationId)
    {
        if (startDelayed)
        {
            print("station chnaged");
            List<WorldIngredient> worldIngreds = new List<WorldIngredient>(FindObjectsByType<WorldIngredient>(FindObjectsSortMode.None));

            foreach (WorldIngredient wIngred in worldIngreds)
            {
                //blank i cant be fucked rn
                
            }
        }
    }

    private IEnumerator WaitAfterStart()
    {
        yield return new WaitForSeconds(0.5f);

        startDelayed = true;
    }
}
