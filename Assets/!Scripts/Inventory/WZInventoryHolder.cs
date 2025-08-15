using System.Collections;
using UnityEngine;

public class WZInventoryHolder : InventoryHolder
{
    [Header("Grid")]
    [SerializeField] private InventoryGridView grid;

    protected override void Awake()
    {
        base.Awake();

        if (!grid) grid = FindFirstObjectByType<InventoryGridView>();
        invBounds = grid.GetComponent<RectTransform>();
        if (grid) grid.BindData(data);
    }

    public void AddNewItem(ModifiedIngredient ingredient) => AddItem(ingredient);
    protected override void OnItemAdded(InventorySlotData slot)
    {
        base.OnItemAdded(slot);
        StartCoroutine(DelayGetVisual(slot));
    }
    public void RemoveItemPublic(ModifiedIngredient ingredient) => RemoveItem(ingredient);

    private IEnumerator DelayGetVisual(InventorySlotData slot)
    {
        yield return new WaitForSeconds(0.5f);
        GameObject visual = grid.GetVisualForSlot(slot);
        if (visual == null) yield break;

        visual = visual.transform.parent.gameObject;
        if (visual != null && visual.TryGetComponent(out InventorySlotScript slotScript))
        {
            slotScript.ActiveInWZ();
        }
    }
}
