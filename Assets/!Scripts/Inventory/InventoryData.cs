using System;
using System.Collections.Generic;

[Serializable]
public class InventorySlotData : IComparable<InventorySlotData>
{
    public IngredientSO ingredient;
    public int amount;

    public InventorySlotData(IngredientSO ingredient, int amount)
    {
        this.ingredient = ingredient;
        this.amount = amount;
    }

    public int CompareTo(InventorySlotData other)
    {
        if (other == null) return 1;
        return ingredient.ID.CompareTo(other.ingredient.ID);
    }
}

[Serializable]
public class InventoryData //hopefully this is good i dont really know what im doing
{
    public enum InventoryType
    {
        AllSlotsVisible,
        OnlyFilledSlots
    }

    //config
    public InventoryType type = InventoryType.AllSlotsVisible;
    public int maxAmountOfItems = 15;
    public int maxDifferentItems = 10;

    //state
    public readonly List<InventorySlotData> slots = new List<InventorySlotData>();

    //events
    public event Action<InventorySlotData> OnItemAdded;
    public event Action<InventorySlotData> OnItemAmountChanged;
    public event Action<InventorySlotData> OnItemRemoved;
    public event Action OnSorted;

    public int TotalAmountCarrying()
    {
        int total = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            total += slots[i].amount;
        }

        return total;
    }

    public int TotalDifferentItems() => slots.Count;

    public InventorySlotData FindSlotByIngredient(IngredientSO ingredient)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].ingredient == ingredient)
            {
                return slots[i];
            }
        }

        return null;
    }

    public bool CanAddOne() => TotalAmountCarrying() < maxAmountOfItems &&
                                TotalDifferentItems() <= maxDifferentItems;

    public bool AddOne(IngredientSO ingredient)
    {
        if (!CanAddOne()) return false;

        //check stack
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].ingredient.ID == ingredient.ID)
            {
                slots[i].amount += 1;
                OnItemAmountChanged?.Invoke(slots[i]);
                return true;
            }
        }

        //new stack
        InventorySlotData slot = new InventorySlotData(ingredient, 1);
        slots.Add(slot);
        OnItemAdded?.Invoke(slot);
        return true;
    }

    public bool RemoveOne(IngredientSO ingredient)
    {
        if (TotalAmountCarrying() <= 0) return false;

        for (int i = slots.Count - 1; i >= 0; i--)
        {
            InventorySlotData slot = slots[i];
            if (slot.ingredient.ID != ingredient.ID) continue;

            if (slot.amount > 1)
            {
                slot.amount -= 1;
                OnItemAmountChanged?.Invoke(slot);
            }
            else
            {
                slots.RemoveAt(i);
                OnItemRemoved?.Invoke(slot);
            }

            return true;
        }

        return false;
    }

    public bool RemoveOne(IngredientSO ingredient, out bool removedStack)
    {
        removedStack = false;
        if (TotalAmountCarrying() <= 0) return false;

        for (int i = slots.Count - 1; i >= 0; i--)
        {
            InventorySlotData slot = slots[i];
            if (slot.ingredient.ID != ingredient.ID) continue;

            if (slot.amount > 1)
            {
                slot.amount -= 1;
                OnItemAmountChanged?.Invoke(slot);
            }
            else
            {
                slots.RemoveAt(i);
                removedStack = true;
                OnItemRemoved?.Invoke(slot);
            }

            return true;
        }

        return false;
    }

    public void Sort()
    {
        slots.Sort();
        OnSorted?.Invoke();
    }
}