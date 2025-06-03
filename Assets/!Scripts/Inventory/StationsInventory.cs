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

    [Header("Triggers")]
    [SerializeField] private TriggerForwarder basketsTrigger;

    [Header("Positioning")]
    [SerializeField] private float yPos;
    [SerializeField] private float zPos;

    [Header("Prefabs")]
    [SerializeField] private GameObject basketPrefab;
    [SerializeField] private GameObject worldItemBase;

    [Header("Station Canvases")]
    [SerializeField] private CraftingRectArea[] craftingRects;
    public CraftingRectArea[] GetCraftingRects()
    {
        return craftingRects;
    }

    [SerializeField] private int _destroySectionIndex = 0;
    public int DestroySectionIndex => _destroySectionIndex;

    //private vars
    private List<Transform> baskets = new List<Transform>();
    private List<BasketItems> basketItems = new List<BasketItems>();

    private StationManager stationManger;

    private List<WorldIngredient> waitingToAdd = new List<WorldIngredient>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        CreateBaskets();
        AddItemsToBaskets();
    }

    void Start()
    {
        stationManger = StationManager.Instance;
        if (stationManger != null) stationManger.OnStationChanged.AddListener(OnStationChangedHandler);

        //setup triggers
        basketsTrigger.onTriggerEnter += AddItemTrigger;
        basketsTrigger.onTriggerExit += RemoveItemTrigger;
    }

    void Update()
    {
        CheckWaitingIngrediant();
    }

    private void CreateBaskets()
    {
        int amountDiff = CheckAmountDifferent();
        for (int i = 0; i < amountDiff; i++)
        {
            float x = (i - (amountDiff / 2)) * spacing;
            if (amountDiff % 2 == 0) x += spacing / 2f;
            Vector3 pos = new Vector3(x, yPos, zPos);

            GameObject newBasket = Instantiate(basketPrefab, pos, Quaternion.identity);
            baskets.Add(newBasket.transform);
        }
    }

    private void AddItemsToBaskets()
    {
        //create locations paired with basketPositions
        int amountDiff = CheckAmountDifferent();
        for (int i = 0; i < amountDiff; i++)
        {
            //check if first item
            if (basketItems.Count <= 0)
            {
                basketItems.Add(new BasketItems(baskets[0], PersistantItemList.inventorySlots[0].ingredient, PersistantItemList.inventorySlots[0].ingredientAmt));
            }
            //find unused basket
            else
            {
                Transform unassignedBasket = null;
                for (int b = 0; b < baskets.Count; b++)
                {
                    bool isUsed = false;
                    for (int j = 0; j < basketItems.Count; j++)
                    {
                        if (basketItems[j].basket == baskets[b])
                        {
                            isUsed = true;
                            break;
                        }
                    }
                    if (!isUsed)
                    {
                        unassignedBasket = baskets[b];
                        break;
                    }
                }

                if (unassignedBasket != null)
                {
                    IngredientSO ingredient = PersistantItemList.inventorySlots[basketItems.Count].ingredient;
                    int count = PersistantItemList.inventorySlots[basketItems.Count].ingredientAmt;
                    basketItems.Add(new BasketItems(unassignedBasket, ingredient, count));
                }
            }
        }

        //Add Objects Visually
        foreach (BasketItems bItem in basketItems)
        {
            for (int i = 0; i < bItem.itemAmount; i++)
            {
                GameObject worldItem = Instantiate(worldItemBase, bItem.basket.position, Quaternion.identity);
                worldItem.GetComponent<WorldIngredient>().ingredient = bItem.assignedIngredient;
                bItem.itemAmount -= 1; //bad way to fix the trigger getting hit on setup but it works
            }
        }
    }

    //Utility
    private int CheckAmountDifferent()
    {
        int amountDiff = 0;
        List<IngredientSO> checkedIngredients = new List<IngredientSO>();

        foreach (InventorySlot item in PersistantItemList.inventorySlots)
        {
            if (!checkedIngredients.Contains(item.ingredient))
            {
                checkedIngredients.Add(item.ingredient);
                amountDiff += 1;
            }
        }

        return amountDiff;
    }

    private void AddItemTrigger(Collider collision)
    {
        /*print("add trigger");
        WorldIngredient worldIngredient = collision.gameObject.GetComponent<WorldIngredient>();
        if (worldIngredient != null)
        {
            foreach (BasketItems bItem in basketItems)
            {
                if (bItem.assignedIngredient == worldIngredient.ingredient)
                {
                    bItem.itemAmount += 1;

                    worldIngredient._isDragging = false;
                    collision.gameObject.transform.position = bItem.basket.position;

                    Rigidbody tempRb = worldIngredient.gameObject.GetComponent<Rigidbody>();
                    tempRb.linearVelocity = Vector3.zero;
                    tempRb.angularVelocity = Vector3.zero;

                    worldIngredient.startPos = bItem.basket.position;

                    break;
                }
            }
        }*/

        WorldIngredient worldIngredient = collision.gameObject.GetComponent<WorldIngredient>();
        if (worldIngredient != null && !waitingToAdd.Contains(worldIngredient))
        {
            foreach (BasketItems bItem in basketItems)
            {
                if (bItem.assignedIngredient == worldIngredient.ingredient)
                {
                    bItem.itemAmount += 1;
                    waitingToAdd.Add(worldIngredient);
                }
            }
        }
    }

    private void CheckWaitingIngrediant()
    {
        if (waitingToAdd.Count <= 0) return;

        List<WorldIngredient> toRemove = new List<WorldIngredient>();

        foreach (WorldIngredient ingred in waitingToAdd)
        {
            if (basketsTrigger.GetComponent<BoxCollider>().bounds.Contains(ingred.transform.position))
            {
                if (!ingred._isDragging)
                {
                    foreach (BasketItems bItem in basketItems)
                    {
                        if (bItem.assignedIngredient == ingred.ingredient)
                        {
                            Rigidbody tempRb = ingred.gameObject.GetComponent<Rigidbody>();
                            tempRb.linearVelocity = Vector3.zero;
                            tempRb.angularVelocity = Vector3.zero;

                            ingred.startPos = bItem.basket.position;
                            ingred.transform.position = bItem.basket.position;

                            toRemove.Add(ingred);
                            break;
                        }
                    }
                }
            }
            else
            {
                toRemove.Add(ingred);
            }
        }

        foreach (var ingred in toRemove)
        {
            waitingToAdd.Remove(ingred);
        }
    }

    private void RemoveItemTrigger(Collider collision)
    {
        WorldIngredient worldIngredient = collision.gameObject.GetComponent<WorldIngredient>();
        if (worldIngredient != null)
        {
            foreach (BasketItems bItem in basketItems)
            {
                if (bItem.assignedIngredient == worldIngredient.ingredient)
                {
                    bItem.itemAmount -= 1;
                    break;
                }
            }
        }
    }

    public void PermanentRemove(WorldIngredient ingredient)
    {
        foreach (BasketItems bItem in basketItems)
        {
            if (bItem.assignedIngredient == ingredient.ingredient)
            {
                bItem.itemAmount -= 1;
                break;
            }
        }

        for (int i = 0; i < PersistantItemList.inventorySlots.Count; i++)
        {
            InventorySlot slot = PersistantItemList.inventorySlots[i];
            if (slot.ingredient == ingredient.ingredient)
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
        print("station changed");
        WorldIngredient[] worldIngreds = FindObjectsByType<WorldIngredient>(FindObjectsSortMode.None);

        List<WorldIngredient> toBeReturned = new List<WorldIngredient>();
        foreach (WorldIngredient ingred in worldIngreds)
        {
            foreach (BasketItems basketItem in basketItems)
            {
                if (ingred.ingredient == basketItem.assignedIngredient)
                {
                    if (!basketsTrigger.GetComponent<BoxCollider>().bounds.Contains(ingred.transform.position)) toBeReturned.Add(ingred);
                    break;
                }
            }
        }

        for (int i = 0; i < toBeReturned.Count; i++)
        {
            float xPos = basketsTrigger.GetComponent<BoxCollider>().bounds.min.x + 2 * (i + 1);
            toBeReturned[i].transform.position = new Vector3(xPos, yPos, zPos);
        }
    }
}
