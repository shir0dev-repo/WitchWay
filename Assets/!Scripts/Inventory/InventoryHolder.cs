using UnityEngine;

public abstract class InventoryHolder : MonoBehaviour
{
    [Header("Persitence")]
    [SerializeField] private bool populateFromPersistantList = true;
    [SerializeField] private bool writeToPersistantList = true;

    public InventoryData data { get; private set; } = new InventoryData();

    [Header("Bounds")]
    [SerializeField] private RectTransform invBounds;
    public RectTransform InvBounds => invBounds;

    protected virtual void Awake()
    {
        //subscribe
        data.OnItemAdded += OnItemAdded;
        data.OnItemAmountChanged += OnItemAmountChanged;
        data.OnItemRemoved += OnItemRemoved;
        data.OnSorted += OnSorted;
    }

    protected virtual void Start()
    {
        if (populateFromPersistantList) PopulateFromStored();
        AfterPopulate();
    }

    protected virtual void OnDestroy()
    {
        data.OnItemAdded -= OnItemAdded;
        data.OnItemAmountChanged -= OnItemAmountChanged;
        data.OnItemRemoved -= OnItemRemoved;
        data.OnSorted -= OnSorted;
    }

    //save and load
    protected virtual void PopulateFromStored()
    {
        foreach (InventorySlotData stored in PersistantItemList.inventorySlots)
        {
            InventorySlotData slot = new InventorySlotData(stored.ingredient, stored.amount);
            data.slots.Add(slot);

            OnItemAdded(slot);
            OnItemAmountChanged(slot);
        }
    }

    protected void UpsertStored(InventorySlotData slot)
    {
        if (!writeToPersistantList) return;

        int index = PersistantItemList.inventorySlots.FindIndex(s => s.ingredient.ID == slot.ingredient.ID);
        if (index >= 0) PersistantItemList.inventorySlots[index].amount = slot.amount;
        else PersistantItemList.inventorySlots.Add(new InventorySlotData(slot.ingredient, slot.amount));
    }

    protected void RemoveFromStored(InventorySlotData slot)
    {
        if (!writeToPersistantList) return;

        int index = PersistantItemList.inventorySlots.FindIndex(s => s.ingredient.ID == slot.ingredient.ID);
        if (index >= 0) PersistantItemList.inventorySlots.RemoveAt(index);
    }

    protected void MirrorOrderToStored()
    {
        if (!writeToPersistantList) return;

        PersistantItemList.inventorySlots.Clear();
        PersistantItemList.inventorySlots.AddRange(data.slots);
    }

    //other systems stuff
    public void AddItem(IngredientSO ingredient) => data.AddOne(ingredient);

    public bool RemoveItem(IngredientSO ingredient) => data.RemoveOne(ingredient);
    public bool RemoveItem(IngredientSO ingredient, out bool removedStack) => data.RemoveOne(ingredient, out removedStack);

    public void SortInventory() => data.Sort();

    //subclass hooks
    protected virtual void AfterPopulate()
    {

    }

    protected virtual void OnItemAdded(InventorySlotData slot)
    {
        UpsertStored(slot);
    }

    protected virtual void OnItemAmountChanged(InventorySlotData slot)
    {
        UpsertStored(slot);
    }

    protected virtual void OnItemRemoved(InventorySlotData slot)
    {
        RemoveFromStored(slot);
    }

    protected virtual void OnSorted()
    {
        MirrorOrderToStored();
    }
}
