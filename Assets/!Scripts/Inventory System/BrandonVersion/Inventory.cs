using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
TO-DO:
 - make type 2 inventory
 - maybe make tpye 3
 - add sort function
*/

[System.Serializable]
public class InventorySlot
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
}

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
    [SerializeField] private Transform slotsGrid;

    [Header("Prefabs")]
    [SerializeField] private GameObject slotVisualObject;

    [Header("testing")]
    [SerializeField] private IngredientSO testIngred;
    [SerializeField] private IngredientSO testIngred2;

    private List<GameObject> slotParents = new List<GameObject>();
    private List<InventorySlot> slotItems = new List<InventorySlot>();

    void Awake()
    {
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
        AddNewItem(testIngred);
        AddNewItem(testIngred2);
        AddNewItem(testIngred2);
        AddNewItem(testIngred);
        AddNewItem(testIngred);

        RemoveItem(testIngred2);
    }

    public void AddNewItem(IngredientSO newIngredient)
    {
        //check if already at max amount
        if (CheckAmountCarrying() >= maxAmountofItems) return;

        //check item doesnt already exist
        bool matchFound = false;
        if (newIngredient.MaxStackSize > 1)
        {
            foreach (InventorySlot slot in slotItems)
            {
                if (slot.ingredient.ID == newIngredient.ID && slot.ingredientAmt < newIngredient.MaxStackSize)
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
        }

        if (!matchFound)
        {
            slotItems.Add(CreateNewIngredientSlot(newIngredient));
        }
    }

    public void RemoveItem(IngredientSO ingredientToRemove)
    {
        //if called when not carrying anything ignore
        if (CheckAmountCarrying() <= 0) return;

        //reverse list
        List<InventorySlot> revList = new List<InventorySlot>(slotItems);
        revList.Reverse();

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
                    slot.slotObject.transform.SetParent(null); //objects destroyed at end of fram so this is required for the realignment
                    Destroy(slot.slotObject);
                    slotItems.Remove(slot);
                    removed = true;
                    break;
                }
            }
        }

        //remove empty spaces to the side
        if (removed)
        {
            for (int i = 0; i < slotParents.Count - 1; i++)
            {
                if (slotParents[i].transform.childCount == 0 && slotParents[i + 1].transform.childCount >= 1)
                {
                    Transform movingChild = slotParents[i + 1].transform.GetChild(0);
                    movingChild.SetParent(slotParents[i].transform);

                    //update InventorySlot item
                    foreach (InventorySlot slot in slotItems)
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

    //Create empty slot
    private void CreateEmptySlot(int slotNum)
    {
        GameObject newSlot = new GameObject();
        newSlot.name = "Slot" + slotNum;
        newSlot.AddComponent<Image>();
        newSlot.GetComponent<RectTransform>().SetParent(slotsGrid, false);
        newSlot.SetActive(true);

        slotParents.Add(newSlot);
    }

    private InventorySlot CreateNewIngredientSlot(IngredientSO newIngredient)
    {
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
        if (slotItems.Count > 0)
        {
            foreach (InventorySlot item in slotItems)
            {
                carriedAmount += item.ingredientAmt;
            }
        }

        return carriedAmount;
    }

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
}
