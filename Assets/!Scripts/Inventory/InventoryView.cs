using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//idk how cleaned up this is
public class InventoryView : InventoryHolder
{
    [Header("Grid")]
    [SerializeField] private InventoryGridView grid;

    [Header("Triggers")]
    [SerializeField] private TriggerForwarder addTrigger;

    protected override void Awake()
    {
        base.Awake();

        if (!grid) grid = FindFirstObjectByType<InventoryGridView>();
        if (grid) grid.BindData(data);
    }

    protected override void Start()
    {
        base.Start();

        if (addTrigger != null) addTrigger.onTriggerEnter += AddItemTrigger;
    }

    public void AddNewItem(ModifiedIngredient ingredient) => AddItem(ingredient);
    public void RemoveItemPublic(ModifiedIngredient ingredient) => RemoveItem(ingredient);

    //add trigger
    public void AddItemTrigger(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out WorldIngredient worldIngredient))
        {
            AddNewItem(worldIngredient.ModifiedState);
            if (CursorManager.Instance != null)
                CursorManager.Instance.ClearCursor(false);
            Destroy(collision.gameObject);
        }
    }
}
