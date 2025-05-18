using System.Collections.Generic;
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

public class InventorySlot
{
    public Transform slotObject;
    public IngredientSO ingrediant;
    public int ingrediantAmt;

    public InventorySlot(IngredientSO ingredient, int ingrediantAmt)
    {
        this.ingrediant = ingredient;
        this.ingrediantAmt = ingrediantAmt;
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

    private List<GameObject> slotParents = new List<GameObject>();

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

    private void CreateEmptySlot(int slotNum)
    {
        GameObject newSlot = new GameObject();
        newSlot.name = "Slot" + slotNum;
        newSlot.AddComponent<Image>();
        newSlot.GetComponent<RectTransform>().SetParent(slotsGrid, false);
        newSlot.SetActive(true);

        slotParents.Add(newSlot);
    }
}
