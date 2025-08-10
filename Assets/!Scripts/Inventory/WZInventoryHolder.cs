using UnityEngine;

public class WZInventoryHolder : InventoryHolder
{
    [Header("Grid")]
    [SerializeField] private InventoryGridView grid;

    protected override void Awake()
    {
        base.Awake();

        if (!grid) grid = FindFirstObjectByType<InventoryGridView>();
        if (grid) grid.BindData(data);
    }

    public void AddNewItem(IngredientSO ingredient) => AddItem(ingredient);
    protected override void OnItemAdded(InventorySlotData slot)
    {
        base.OnItemAdded(slot);

        GameObject visual = grid.GetVisualForSlot(slot);
        if (visual != null)
        {
            visual.GetComponent<InventorySlotScript>()?.ActiveInWZ();
        }
    }
    public void RemoveItemPublic(IngredientSO ingredient) => RemoveItem(ingredient);
}
