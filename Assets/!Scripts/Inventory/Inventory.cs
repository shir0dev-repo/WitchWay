using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//NOTE: if max item amount changes will need to add functionality to rescale items

[System.Serializable]
public class InventorySlot : IComparable<InventorySlot>
{
    public Transform slotParent;
    public GameObject slotObject;
    public IngredientSO ingredient;
    public int ingredientAmt = 0;

    public InventorySlot(IngredientSO ingredient, int ingredientAmt)
    {
        this.ingredient = ingredient;
        this.ingredientAmt = ingredientAmt;
    }

    public int CompareTo(InventorySlot other)
    {
        if (other == null) return 1;
        return this.ingredient.ID.CompareTo(other.ingredient.ID);
    }
}

public class Inventory : MonoBehaviour
{
    private enum InventoryType
    {
        AllSlotsVisible, //shows {number} slots even if items stack
        OnlyFilledSlots
    }

    [Header("Inventory Settings")]
    [SerializeField] private InventoryType inventoryType = InventoryType.AllSlotsVisible;
    [SerializeField] private int maxAmountofItems = 15; //slots amt can possibly be made interchangable for this instead of seperate var 
    [SerializeField] private int maxDifferentItems = 10;
    [SerializeField] private int amountOfSlots; // only applies when type is all slots visible (write editor code to hide?)

    [Header("Parent Transforms")]
    [SerializeField] private Transform slotsGrid;

    [Header("Prefabs")]
    [SerializeField] private GameObject slotVisualObject;

    [Header("Triggers")]
    [SerializeField] private TriggerForwarder addTrigger;

    private List<GameObject> slotParents = new List<GameObject>();
    //private List<InventorySlot> slotItems = new List<InventorySlot>();

    void Awake()
    {
        //Create full inventory of slots
        if (inventoryType == InventoryType.AllSlotsVisible)
        {
            for (int i = 0; i < amountOfSlots; i++)
            {
                CreateEmptySlot(i);
            }
        }
    }

    void Start()
    {
        if(addTrigger != null) addTrigger.onTriggerEnter += AddItemTrigger;

        //check if already populated
        if (PersistantItemList.inventorySlots.Count > 0)
        {
            foreach (InventorySlot slot in PersistantItemList.inventorySlots)
            {
                AddNewItem(slot.ingredient);
            }
        }
    }

    //Add new ingrediant to be displayed and stored in inventory
    public void AddNewItem(IngredientSO newIngredient)
    {
        //check if already at max amount
        if (CheckAmountCarrying() >= maxAmountofItems || CheckAmountDifferent() > maxDifferentItems) return;

        //check item doesnt already exist
        bool matchFound = false;

        foreach (InventorySlot slot in PersistantItemList.inventorySlots)
        {
            if (slot.ingredient.ID == newIngredient.ID)
            {
                slot.ingredientAmt += 1;
                matchFound = true;

                if (slot.ingredientAmt > 1)
                {
                    slot.slotObject.GetComponentInChildren<TMP_Text>().text = slot.ingredientAmt.ToString();
                }

                break;
            }
        }

        //if item doesnt already exist or stack is full/non-stackable create new one
        if (!matchFound)
        {
            PersistantItemList.inventorySlots.Add(CreateNewIngredientSlot(newIngredient));
        }
    }

    //Remove an item from inventory
    public void RemoveItem(IngredientSO ingredientToRemove)
    {
        //if called when not carrying anything ignore
        if (CheckAmountCarrying() <= 0) return;

        //reverse list
        List<InventorySlot> revList = new List<InventorySlot>(PersistantItemList.inventorySlots);
        revList.Reverse();

        //decrese item amount or remove it
        bool removed = false;
        foreach (InventorySlot slot in revList)
        {
            if (slot.ingredient.ID == ingredientToRemove.ID)
            {
                if (slot.ingredientAmt > 1)
                {
                    slot.ingredientAmt -= 1;
                    break;
                }
                else
                {
                    slot.slotObject.transform.SetParent(null); //objects destroyed at end of frame so this is required for the realignment
                    Destroy(slot.slotObject);
                    if (inventoryType == InventoryType.OnlyFilledSlots) Destroy(slot.slotParent.gameObject);

                    PersistantItemList.inventorySlots.Remove(slot);
                    removed = true;
                    break;
                }
            }
        }

        //remove empty spaces to the side
        if (removed && inventoryType == InventoryType.AllSlotsVisible)
        {
            for (int i = 0; i < slotParents.Count - 1; i++)
            {
                if (slotParents[i].transform.childCount == 0 && slotParents[i + 1].transform.childCount >= 1)
                {
                    Transform movingChild = slotParents[i + 1].transform.GetChild(0);
                    movingChild.SetParent(slotParents[i].transform);

                    //update InventorySlot item
                    foreach (InventorySlot slot in PersistantItemList.inventorySlots)
                    {
                        if (slot.slotObject == movingChild)
                        {
                            slot.slotParent = slotParents[i].transform;
                            break;
                        }
                    }

                    i--;
                }
            }
        }
    }

    //sorts items by lowest id number first
    public void SortInventory()
    {
        //sort itmes in slotItems list
        PersistantItemList.inventorySlots.Sort();

        //update gameobject positions
        for (int i = 0; i < PersistantItemList.inventorySlots.Count; i++)
        {
            if (PersistantItemList.inventorySlots[i].slotObject != null)
            {
                PersistantItemList.inventorySlots[i].slotObject.transform.SetParent(slotParents[i].transform, false);
                PersistantItemList.inventorySlots[i].slotParent = slotParents[i].transform;
            }
        }
    }

    //Create empty slot
    private void CreateEmptySlot(int slotNum)
    {
        GameObject newSlot = new GameObject(); //this can be replaced later with a prefab
        newSlot.name = "Slot" + slotNum;
        newSlot.AddComponent<Image>();
        newSlot.GetComponent<RectTransform>().SetParent(slotsGrid, false);
        newSlot.GetComponent<RectTransform>().sizeDelta = new Vector2(245, 245);
        newSlot.SetActive(true);

        slotParents.Add(newSlot);
    }

    //create a new slot for ingrediants
    private InventorySlot CreateNewIngredientSlot(IngredientSO newIngredient)
    {
        //no check for max items as this is never called if we are at max
        Transform parentTransform = null;
        if (inventoryType == InventoryType.AllSlotsVisible) parentTransform = FindEmptySlot();

        InventorySlot newItemSlot = new InventorySlot(newIngredient, 1);
        if (parentTransform == null)
        {
            CreateEmptySlot(slotParents.Count);
            parentTransform = FindEmptySlot();
        }
        newItemSlot.slotParent = parentTransform;

        GameObject itemSlotVisual = Instantiate(slotVisualObject, parentTransform);
        itemSlotVisual.GetComponentInChildren<Image>().sprite = newIngredient.Sprite;
        newItemSlot.slotObject = itemSlotVisual;

        return newItemSlot;
    }

    //check amount in inventory
    private int CheckAmountCarrying()
    {
        int carriedAmount = 0;
        if (PersistantItemList.inventorySlots.Count > 0)
        {
            foreach (InventorySlot item in PersistantItemList.inventorySlots)
            {
                carriedAmount += item.ingredientAmt;
            }
        }

        return carriedAmount;
    }

    //check how many different items there are
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

    //Find an empty slot, used in AllSlotsVisible
    private Transform FindEmptySlot()
    {
        Transform emptySlot = null;
        foreach (GameObject slot in slotParents)
        {
            if (slot.transform.childCount == 0)
            {
                emptySlot = slot.transform;
                break;
            }
        }

        return emptySlot;
    }

    public void AddItemTrigger(Collider collision)
    {
        WorldIngredient worldIngredient = collision.gameObject.GetComponent<WorldIngredient>();
        if (worldIngredient != null)
        {
            AddNewItem(worldIngredient.BaseIngredient);
        }
    }
}
