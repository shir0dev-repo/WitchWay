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

public class StationsInventory : MonoBehaviour
{
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

    //private vars
    private List<Transform> baskets = new List<Transform>();
    private List<BasketItems> basketItems = new List<BasketItems>();

    void Awake()
    {
        CreateBaskets();
        AddItemsToBaskets();
    }

    void Start()
    {
        //setup triggers
        basketsTrigger.onTriggerEnter += AddItemTrigger;
        basketsTrigger.onTriggerExit += RemoveItemTrigger;
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

                    break;
                }
            }
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
}
