using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

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
        obj.GetComponentInChildren<Image>(true).sprite = slot.ingredient.BaseIngredient.Sprite;

        TMP_Text text = obj.GetComponentInChildren<TMP_Text>(true);
        if (text) text.text = slot.amount > 1 ? slot.amount.ToString() : "";

        InventorySlotScript slotScript = parent.GetComponent<InventorySlotScript>();
        if (slotScript)
        {
            slotScript.SetIngredient(slot.ingredient);

            slotScript.BindSlot(slot);
        }

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
        
        RectTransform rt = slot.AddComponent<RectTransform>();
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

    public GameObject GetVisualForSlot(InventorySlotData slot)
    {
        if (slotPartners.TryGetValue(slot, out GameObject go))
            return go;
        return null;
    }

    //dragging handling
    //this shit sucks dont ask me how it works it just does
    [Header("Dragging")]
    [SerializeField] private Canvas aboveCanvas;

    private GameObject placeholder;
    private RectTransform placeholderRT;
    private GameObject draggingObj;
    private RectTransform draggingRT;
    private Transform originalParent;
    private int originalIndex = -1;
    private LayoutElement draggingLE;

    public void BeginDrag(InventorySlotData slot, GameObject slotObj, PointerEventData evt)
    {
        if (draggingObj != null) EndDrag(true);

        draggingObj = slotObj;
        draggingRT = slotObj.GetComponent<RectTransform>();
        originalParent = slotObj.transform.parent;
        originalIndex = slotObj.transform.GetSiblingIndex();

        //create placeholder
        placeholder = new GameObject("PlaceHolder");
        placeholderRT = placeholder.AddComponent<RectTransform>();
        placeholderRT.SetParent(originalParent, false);
        placeholderRT.SetSiblingIndex(originalIndex);
        placeholderRT.sizeDelta = draggingRT.sizeDelta;

        //lift
        draggingObj.transform.SetParent(aboveCanvas.transform, true);

        draggingLE = draggingObj.GetComponent<LayoutElement>();
        if (draggingLE == null) draggingLE = draggingObj.AddComponent<LayoutElement>();
        draggingLE.ignoreLayout = true;

        UpdateDraggingPosition(evt);
        UpdatePlaceholderPosition(evt);
    }

    public void Drag(PointerEventData evt)
    {
        if (!draggingObj) return;

        UpdateDraggingPosition(evt);
        UpdatePlaceholderPosition(evt);
    }

    public void EndDrag(bool droppedOutside)
    {
        if (!draggingObj) return;

        if (draggingLE) draggingLE.ignoreLayout = false;

        if (!droppedOutside && placeholderRT != null && placeholderRT.parent != null)
        {
            int newIndex = placeholderRT.GetSiblingIndex();
            draggingObj.transform.SetParent(placeholderRT.parent, true);
            draggingObj.transform.SetSiblingIndex(newIndex);

            //do we ned to update data to reflect chnaged order?
        }
        else
        {
            draggingObj.transform.SetParent(originalParent, true);
            draggingObj.transform.SetSiblingIndex(originalIndex);
        }

        //cleaning time
        if (placeholder) Destroy(placeholder);
        placeholder = null; placeholderRT = null; draggingObj = null; draggingRT = null; originalIndex = -1; draggingLE = null;
    }

    private void UpdateDraggingPosition(PointerEventData evt)
    {
        RectTransform canvasRT = aboveCanvas.transform as RectTransform;
        Camera cam = aboveCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : aboveCanvas.worldCamera;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, evt.position, cam, out Vector2 local))
        {
            draggingRT.localPosition = local;
        }
    }

    private void UpdatePlaceholderPosition(PointerEventData evt)
    {
        if (!slotsGrid) return; //should never happen but safty

        int targetindex = ComputeInsertIndex(evt.position);
        if (targetindex < 0) targetindex = slotsGrid.childCount;

        targetindex = Mathf.Clamp(targetindex, 0, slotsGrid.childCount);
        if (placeholderRT != null && placeholderRT.parent == slotsGrid)
        {
            placeholderRT.SetSiblingIndex(targetindex);
        }
    }

    private int ComputeInsertIndex(Vector2 screenPos) //insert spot decider
    {
        RectTransform gridRT = slotsGrid as RectTransform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRT, screenPos, null, out Vector2 localPoint)) return -1;

        List<RectTransform> children = Enumerable.Range(0, slotsGrid.childCount)
                                    .Select(i => slotsGrid.GetChild(i) as RectTransform)
                                    .Where(rt => rt != null)
                                    .ToList(); //craxy

        if (children.Count == 0) return 0;

        //find closest
        int bestIndex = children.Count - 1;
        float best = float.MaxValue;

        for (int i = 0; i < children.Count; i++)
        {
            RectTransform child = children[i];
            Vector2 center = child.localPosition;
            float dist = Vector2.SqrMagnitude(localPoint - center);
            if (dist < best)
            {
                best = dist;
                bestIndex = i;
            }
        }

        RectTransform closest = children[bestIndex];
        if (localPoint.x > closest.localPosition.x) bestIndex += 1;

        return bestIndex;
    }
}
