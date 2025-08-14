using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.UIElements;

[System.Serializable]
public class BasketItems
{
    public Transform basket;
    public ModifiedIngredient assignedIngredient;
    public int itemAmount;

    public BasketItems(Transform basket, ModifiedIngredient assignedIngredient, int itemAmount)
    {
        this.basket = basket;
        this.assignedIngredient = assignedIngredient;
        this.itemAmount = itemAmount;
    }
}

[System.Serializable]
public class CraftingRectArea
{
    public RectTransform screenRect;
    public float depthValue;
}

public class StationsInventoryHolder : InventoryHolder
{
    [Header("Basket Settings")]
    [SerializeField] private float spacing;

    [Header("Positioning")]
    [SerializeField] private float yPos;
    [SerializeField] private float zPos;

    [Header("Station Canvases")]
    [SerializeField] private CraftingRectArea[] craftingRects;
    public CraftingRectArea[] GetCraftingRects() => craftingRects;

    [SerializeField] private int _destroySectionIndex = 0;
    public int DestroySectionIndex => _destroySectionIndex;

    [Header("Boxes")]
    [SerializeField] private StationsInvBox[] boxes;

    [Header("Sound")]
    [SerializeField] private EventReference OnItemTrashedSound;

    private bool startDelayed = false;

    private void OnEnable()
    {
        GameEvents.Crafting.OnStationChanged += OnStationChangedHandler;
        GameEvents.Crafting.OnItemPlacedInTrash += PermanentRemove;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnStationChanged -= OnStationChangedHandler;
        GameEvents.Crafting.OnItemPlacedInTrash -= PermanentRemove;
    }

    protected override void AfterPopulate()
    {
        SortIntoBoxes();
        StartCoroutine(WaitAfterStart());
    }

    //station logic
    private void SortIntoBoxes()
    {
        foreach (StationsInvBox box in boxes) box.ClearItems();

        List<BasketItems> items = new List<BasketItems>();
        foreach (InventorySlotData slot in data.slots)
        {
            for (int i = 0; i < slot.amount; i++)
            {
                items.Add(new BasketItems(null, slot.ingredient, 1));
            }
        }

        int[] boxCounts = new int[boxes.Length];
        int currentBox = 0;

        foreach (BasketItems item in items)
        {
            int remaining = item.itemAmount;
            while (remaining > 0)
            {
                int tries = 0;
                while (boxCounts[currentBox] >= 15 && tries < boxes.Length)
                {
                    currentBox = (currentBox + 1) % boxes.Length;
                    tries++;
                }

                if (tries >= boxes.Length)
                {
                    Debug.LogWarning("to many items");
                    return;
                }

                boxes[currentBox].AddItem(new BasketItems(boxes[currentBox].transform, item.assignedIngredient, 1));
                boxCounts[currentBox]++;
                remaining--;
                currentBox = (currentBox + 1) % boxes.Length;
            }
        }
    }

    public void PermanentRemove(WorldIngredient ingredient)
    {
        bool removedStack;
        bool good = RemoveItem(ingredient.ModifiedState, out removedStack);

        if (good)
        {
            SortIntoBoxes();

            if (removedStack)
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayOneShot(OnItemTrashedSound, ingredient.transform.position);
            }
        }
    }

    private void OnStationChangedHandler(int stationId)
    {
        if (startDelayed) StartCoroutine(SendToInventory());
    }

    private IEnumerator WaitAfterStart()
    {
        yield return new WaitForSeconds(0.5f);
        startDelayed = true;
    }

    private IEnumerator SendToInventory()
    {
        WorldIngredient[] ingredientsInWorld = FindObjectsByType<WorldIngredient>(FindObjectsSortMode.None);
        Queue<WorldIngredient> queue = new Queue<WorldIngredient>(ingredientsInWorld);

        int boxIndex = 0;
        StationsInvBox invBox = boxes[boxIndex];

        while (queue.TryDequeue(out WorldIngredient wIng))
        {
            if (wIng.TryGetComponent(out HoverToLocation hover))
            {
                hover.Target = invBox.transform;
                boxIndex = (boxIndex + 1) % boxes.Length;
                invBox = boxes[boxIndex];
            }

            yield return null;
        }
    }

    //parent class hooks
    protected override void OnItemAdded(InventorySlotData slot)
    {
        base.OnItemAdded(slot);

        SortIntoBoxes();
    }

    protected override void OnItemAmountChanged(InventorySlotData slot)
    {
        base.OnItemAmountChanged(slot);
        SortIntoBoxes();
    }

    protected override void OnItemRemoved(InventorySlotData slot)
    {
        base.OnItemRemoved(slot);
        SortIntoBoxes();
    }
}
