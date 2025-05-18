using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
TO-DO:
 - add images to slots
 - add numbers to existing item slots
 - add check for stack size
 - make type 2 inventory
 - maybe make tpye 3
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
        AddNewItem(testIngred);
        AddNewItem(testIngred);
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
            newItemSlot.slotParent = parentTransform;
        }

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
