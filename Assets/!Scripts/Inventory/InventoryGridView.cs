using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryGridView : MonoBehaviour
{
    public enum InventoryType
    {
        AllSlotsVisible,
        OnlyFilledSlots
    }

    [Header("Inventory Settings")]
    [SerializeField] private InventoryType type = InventoryType.OnlyFilledSlots;
    [SerializeField] private int amountOfSlots = 15;

    [Header("References")]
    [SerializeField] private Transform slotsGrid;
    [SerializeField] private GameObject slotVisualObject;

    private readonly List<GameObject> slotParents = new List<GameObject>();
    private readonly Dictionary<InventorySlotData, GameObject> slotPartners = new Dictionary<InventorySlotData, GameObject>();
    private InventoryData data;

    public void BindData(InventoryData data)
    {
        if (this.data != null) UnSubscribe(this.data);
        this.data = data;
        Subscribe(this.data);

        if (type == InventoryType.AllSlotsVisible && slotParents.Count == 0)
        {
            for (int i = 0; i < amountOfSlots; i++) CreateEmptySlot(i);
        }

        RedrawAll();
    }

    private void Subscribe(InventoryData data)
    {
        data.OnItemAdded += OnItemAdded;
        data.OnItemAmountChanged += OnItemAmountChanged;
        data.OnItemRemoved += OnItemRemoved;
        data.OnSorted += OnSorted;
    }

    private void UnSubscribe(InventoryData data)
    {
        data.OnItemAdded -= OnItemAdded;
        data.OnItemAmountChanged -= OnItemAmountChanged;
        data.OnItemRemoved -= OnItemRemoved;
        data.OnSorted -= OnSorted;
    }

    private void RedrawAll()
    {
        foreach (GameObject obj in slotPartners.Values) if (obj) Destroy(obj);
        slotPartners.Clear();

        foreach (InventorySlotData slot in data.slots)
        {
            OnItemAdded(slot);
            OnItemAmountChanged(slot);
        }

        OnSorted();
    }

    //handlers
    private void OnItemAdded(InventorySlotData slot)
    {
        Transform parent = (type == InventoryType.AllSlotsVisible) ? FindEmptySlot() ?? CreateAndReturnSlotParent() : CreateAndReturnSlotParent();

        GameObject obj = Instantiate(slotVisualObject, parent);
        obj.GetComponentInChildren<Image>().sprite = slot.ingredient.Sprite;

        TMP_Text text = obj.GetComponentInChildren<TMP_Text>(true);
        if (text) text.text = slot.amount > 1 ? slot.amount.ToString() : "";

        InventorySlotScript slotScript = parent.GetComponent<InventorySlotScript>();
        if (slotScript) slotScript.SetIngredient(slot.ingredient);

        slotPartners[slot] = obj;
    }

    private void OnItemAmountChanged(InventorySlotData slot)
    {
        if (!slotPartners.TryGetValue(slot, out GameObject obj) || !obj) return;

        TMP_Text text = obj.GetComponentInChildren<TMP_Text>(true);
        if (text) text.text = slot.amount > 1 ? slot.amount.ToString() : "";
    }

    private void OnItemRemoved(InventorySlotData slot)
    {
        if (slotPartners.TryGetValue(slot, out GameObject obj) && obj)
        {
            Transform parent = obj.transform.parent;
            obj.transform.SetParent(null);
            Destroy(obj);

            if (type == InventoryType.OnlyFilledSlots && parent && parent.childCount == 0)
            {
                slotParents.Remove(parent.gameObject);
                Destroy(parent.gameObject);
            }
        }
        slotPartners.Remove(slot);

        if (type == InventoryType.AllSlotsVisible) CompactLeft();
    }

    private void OnSorted()
    {
        if (type != InventoryType.AllSlotsVisible) return;

        for (int i = 0; i < data.slots.Count && i < slotParents.Count; i++)
        {
            InventorySlotData slot = data.slots[i];
            if (slotPartners.TryGetValue(slot, out GameObject obj) && obj) obj.transform.SetParent(slotParents[i].transform, false);
        }
    }

    //helpers
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
}
