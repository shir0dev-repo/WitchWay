using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasketItems
{
    public Transform basket;
    public IngredientSO assignedIngredient;

    public BasketItems(Transform basket, IngredientSO assignedIngredient)
    {
        this.basket = basket;
        this.assignedIngredient = assignedIngredient;
    }
}

public class StationsInventory : MonoBehaviour
{
    [Header("Basket Settings")]
    [SerializeField] private float spacing;

    [Header("Prefabs")]
    [SerializeField] private GameObject basketPrefab;

    //private vars
    private List<Transform> baskets = new List<Transform>();
    private List<BasketItems> basketItems = new List<BasketItems>();

    void Awake()
    {
        CreateBaskets();
        AddItemsToBaskets();
    }

    private void CreateBaskets()
    {
        int amountDiff = CheckAmountDifferent();
        for (int i = 0; i < amountDiff; i++)
        {
            float x = (i - (amountDiff / 2)) * spacing;
            if (amountDiff % 2 == 0) x += spacing / 2f;
            Vector3 pos = new Vector3(x, 0, 0);

            GameObject newBasket = Instantiate(basketPrefab, pos, Quaternion.identity);
            baskets.Add(newBasket.transform);
        }
    }

    private void AddItemsToBaskets()
    {
        int amountDiff = CheckAmountDifferent();
        for (int i = 0; i < amountDiff; i++)
        {
            //check if first item
            if (basketItems.Count <= 0)
            {
                basketItems.Add(new BasketItems(baskets[0], PersistantItemList.inventorySlots[0].ingredient));
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
                    var ingredient = PersistantItemList.inventorySlots[basketItems.Count].ingredient;
                    basketItems.Add(new BasketItems(unassignedBasket, ingredient));
                }
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
}
