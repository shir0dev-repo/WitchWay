using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//idk how cleaned up this is
public class InventoryView : Singleton<InventoryView>
{
    [Header("Inventroy Settings")]
    [SerializeField] private InventoryData.InventoryType inventoryType = InventoryData.InventoryType.AllSlotsVisible;
    [SerializeField] private int maxAmountOfItems = 15;
    [SerializeField] private int maxDifferentItems = 10;
    [SerializeField] private int amountOfSlots;

    [Header("Parent Transforms")]
    [SerializeField] private Transform slotsGrid;

    [Header("Prefabs")]
    [SerializeField] private GameObject slotVisualObject;

    [Header("Triggers")]
    [SerializeField] private TriggerForwarder addTrigger;

    [Header("Data")]
    public InventoryData data { get; private set; } = new InventoryData();

    //other
    private readonly List<GameObject> slotParents = new List<GameObject>();
    private readonly Dictionary<InventorySlotData, GameObject> slotPartner = new Dictionary<InventorySlotData, GameObject>();

    protected override void Awake()
    {
        base.Awake();

        //data config
        data.type = inventoryType;
        data.maxAmountOfItems = maxAmountOfItems;
        data.maxDifferentItems = maxDifferentItems;

        //subscribe
        data.OnItemAdded += HandleItemAdded;
        data.OnItemAmountChanged += HandleItemAmountChanged;
        data.OnItemRemoved += HandleItemRemoved;
        data.OnSorted += HandleSorted;

        //prebuild if needed
        if (data.type == InventoryData.InventoryType.AllSlotsVisible)
        {
            for (int i = 0; i < amountOfSlots; i++)
            {
                CreateEmptySlot(i);
            }
        }
    }

    void Start()
    {
        if (addTrigger != null) addTrigger.onTriggerEnter += AddItemTrigger;

        LoadFromStored();
    }

    //save helpers
    private void LoadFromStored()
    {
        foreach (InventorySlotData saved in PersistantItemList.inventorySlots)
        {
            InventorySlotData slot = new InventorySlotData(saved.ingredient, saved.amount);
            data.slots.Add(slot);

            HandleItemAdded(slot);
            HandleItemAmountChanged(slot);
        }
    }

    private void UpsertStored(InventorySlotData slot)
    {
        int index = PersistantItemList.inventorySlots.FindIndex(s => s.ingredient.ID == slot.ingredient.ID);
        if (index >= 0) PersistantItemList.inventorySlots[index].amount = slot.amount;
        else PersistantItemList.inventorySlots.Add(new InventorySlotData(slot.ingredient, slot.amount));
    }

    private void RemoveFromStored(InventorySlotData slot)
    {
        int index = PersistantItemList.inventorySlots.FindIndex(s => s.ingredient.ID == slot.ingredient.ID);
        if(index >= 0) PersistantItemList.inventorySlots.RemoveAt(index);
    }

    //event handlers
    private void HandleItemAdded(InventorySlotData slot)
    {
        Transform parent = (data.type == InventoryData.InventoryType.AllSlotsVisible) ? FindEmptySlot() ?? CreateAndReturnSlotParent() : CreateAndReturnSlotParent();

        //build visual
        GameObject item = Instantiate(slotVisualObject, parent);
        item.GetComponentInChildren<Image>().sprite = slot.ingredient.Sprite;

        //amount label
        TMP_Text text = item.GetComponentInChildren<TMP_Text>(true);
        if (text != null) text.text = slot.amount > 1 ? slot.amount.ToString() : "";

        InventorySlotScript slotScript = parent.GetComponent<InventorySlotScript>();
        if (slotScript != null) slotScript.SetIngredient(slot.ingredient);

        slotPartner[slot] = item;

        UpsertStored(slot);
    }

    private void HandleItemAmountChanged(InventorySlotData slot)
    {
        if (!slotPartner.TryGetValue(slot, out GameObject obj) || obj == null) return;

        TMP_Text text = obj.GetComponentInChildren<TMP_Text>(true);
        if (text != null) text.text = slot.amount > 1 ? slot.amount.ToString() : "";

        UpsertStored(slot);
    }

    private void HandleItemRemoved(InventorySlotData slot)
    {
        if (!slotPartner.TryGetValue(slot, out GameObject obj) || obj == null) return;

        Transform parent = obj.transform.parent;

        obj.transform.SetParent(null);
        Destroy(obj);
        slotPartner.Remove(slot);

        if (data.type == InventoryData.InventoryType.OnlyFilledSlots)
        {
            if (parent != null && parent.childCount == 0)
            {
                slotParents.Remove(parent.gameObject);
                Destroy(parent.gameObject);
            }
        }
        else
        {
            CompactLeft();
        }

        RemoveFromStored(slot);
    }

    private void HandleSorted()
    {
        if (data.type != InventoryData.InventoryType.AllSlotsVisible) return;

        for (int i = 0; i < data.slots.Count && i < slotParents.Count; i++)
        {
            InventorySlotData slot = data.slots[i];
            if (slotPartner.TryGetValue(slot, out GameObject obj) && obj != null)
            {
                obj.transform.SetParent(slotParents[i].transform, false);
            }
        }

        PersistantItemList.inventorySlots.Clear();
        PersistantItemList.inventorySlots.AddRange(data.slots);
    }

    //called by game
    public void AddNewItem(IngredientSO newIngredient)
    {
        data.AddOne(newIngredient);
    }

    public void RemoveItem(IngredientSO ingredient)
    {
        data.RemoveOne(ingredient);
    }

    public void SortInventory()
    {
        data.Sort();
    }

    //ui helpers
    private void CreateEmptySlot(int number)
    {
        GameObject slot = new GameObject($"Slot{number}");
        slot.AddComponent<Image>();
        RectTransform rt = slot.GetComponent<RectTransform>();
        rt.SetParent(slotsGrid, false);
        rt.sizeDelta = new Vector2(245, 245);

        slot.AddComponent<InventorySlotScript>();
        slot.SetActive(true);
        slotParents.Add(slot);
    }

    private Transform CreateAndReturnSlotParent()
    {
        if (data.type == InventoryData.InventoryType.AllSlotsVisible)
        {
            Transform t = FindEmptySlot();
            if (t != null) return t;

            CreateEmptySlot(slotParents.Count);
            return slotParents[^1].transform;
        }
        else
        {
            CreateEmptySlot(slotParents.Count);
            return slotParents[^1].transform;
        }
    }

    private Transform FindEmptySlot()
    {
        for (int i = 0; i < slotParents.Count; i++)
        {
            if (slotParents[i].transform.childCount == 0) return slotParents[i].transform;
        }
        return null;
    }

    private void CompactLeft()
    {
        for (int i = 0; i < slotParents.Count - 1; i++)
        {
            if (slotParents[i].transform.childCount == 0 && slotParents[i + 1].transform.childCount >= 1)
            {
                Transform movingChild = slotParents[i + 1].transform.GetChild(0);
                movingChild.SetParent(slotParents[i].transform);
                i--;
            }
        }
    }

    //add trigger
    public void AddItemTrigger(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out WorldIngredient worldIngredient))
        {
            AddNewItem(worldIngredient.BaseIngredient);
            if (CursorManager.Instance != null)
                CursorManager.Instance.ClearCursor(false);
            Destroy(collision.gameObject);
        }
    }
}
