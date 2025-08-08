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
    public void RemoveItemPublic(IngredientSO ingredient) => RemoveItem(ingredient);
}
