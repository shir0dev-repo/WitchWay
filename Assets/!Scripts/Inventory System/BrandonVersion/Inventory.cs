using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private enum InventoryType
    {
        AllSlotsVisible, //shows {number} slots even if items stack
        OnlyFilledSlots //shows only slots when filled, scales 
        //third type with diffrent scaling? starts base size
    }

    [Header("Inventory Settings")]
    [SerializeField] private InventoryType inventoryType = InventoryType.AllSlotsVisible;
    [SerializeField] private float maxAmountofItems = 15; //slots amt can possibly be made interchangable for this instead of seperate var
    [SerializeField] private int amountOfSlots; // only applies when type is all slots visible (write editor code to hide)

    [Header("Parent Transforms")]
    [SerializeField] private Transform slotsParent;

    //private vars
    Dictionary<IngredientSO, int> inventorySlots = new Dictionary<IngredientSO, int>(); //ingrediant, amount of

    void Awake()
    {
        if (inventoryType == InventoryType.AllSlotsVisible) CreateAllSlots();
    }

    public void AddItem(IngredientSO newIngrediant)
    {
        if (CheckAmountCarrying() >= maxAmountofItems) return;

        //check if already added
        bool matchFound = false;
        foreach (IngredientSO key in inventorySlots.Keys)
        {
            if (key.ID == newIngrediant.ID) //is ID check nessary? would the keys match?
            {
                inventorySlots[key] += 1;
                matchFound = true;
                break;
            }
        }

        if (!matchFound)
        {
            inventorySlots.Add(newIngrediant, 1);
        }
    }

    public void RemoveItem(IngredientSO ingredientToRemove)
    {
        if (CheckAmountCarrying() <= 0) return;

        foreach (IngredientSO key in inventorySlots.Keys)
        {
            if (key.ID == ingredientToRemove.ID)
            {
                inventorySlots[key] -= 1;

                if (inventorySlots[key] <= 0)
                {
                    inventorySlots.Remove(key);
                }
            }
        }
    }

    //Create all the slots for AllSlotsVisible type
    private void CreateAllSlots()
    {
        for (int i = 0; i < amountOfSlots; i++)
        {
            //This can prob be replaced by a prefab in the future
            GameObject newSlot = new GameObject();
            newSlot.name = "Slot" + i;
            newSlot.AddComponent<Image>();
            newSlot.GetComponent<RectTransform>().SetParent(slotsParent, false);
            newSlot.SetActive(true);
        }
    }

    private int CheckAmountCarrying()
    {
        int totalItems = 0;
        foreach (KeyValuePair<IngredientSO, int> pair in inventorySlots)
        {
            totalItems += pair.Value;
        }

        return totalItems;
    }
}
