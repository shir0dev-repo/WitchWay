using System.Collections.Generic;

namespace Shir0.InventorySystem
{
    /// <summary>
    /// A collection of <see cref="InventorySlot"/> with a specified size.
    /// </summary>
    [System.Serializable]
    public class Inventory
    {
        private readonly int m_size;
        [UnityEngine.SerializeField] private InventorySlot[] m_slots;

        /// <summary>
        /// Collection of slots owned by this inventory.
        /// </summary>
        public InventorySlot[] Slots
        {
            get { return m_slots; }
            private set { m_slots = value; }
        }

        /// <summary>
        /// Size of the inventory.
        /// </summary>
        public int Size
        {
            get { return m_size; }
        }

        /// <summary>
        /// The amount of slots with no assigned item.
        /// </summary>
        public int SlotsRemaining
        {
            get
            {
                int slotCount = 0;
                foreach (InventorySlot slot in m_slots)
                {
                    if (slot.CurrentItem == null)
                        slotCount++;
                }

                return slotCount;
            }
        }

        public InventorySlot this[int i]
        {
            get { return m_slots[i]; }
            private set { m_slots[i] = value; }
        }

        /// <summary>
        /// A new inventory with a specified size.
        /// </summary>
        /// <param name="size">Size of the inventory.</param>
        /// <exception cref="InvalidInventoryOperationException"/>
        public Inventory(int size)
        {
            if (size < 1)
                throw new InvalidInventoryOperationException("Inventory cannot be size zero!");

            m_size = size;
            m_slots = new InventorySlot[size];

            for (int i = 0; i < size; i++)
                m_slots[i] = new InventorySlot();

        }

        /// <summary>
        /// Constructor for copying one inventory to a newly created one.
        /// </summary>
        /// <param name="slotsOther">Slot array to copy from.</param>
        public Inventory(InventorySlot[] slotsOther)
        {
            m_slots = slotsOther;
            m_size = slotsOther.Length;
        }

        /// <summary>
        /// Adds a specified <paramref name="amount"/> of <see cref="ItemData"/> to the <see cref="Inventory"/>.
        /// </summary>
        /// <param name="itemCountTuple">The item and amount to add.</param>
        /// <returns>
        /// <see langword="true"/>: Item was fully added to inventory.<br/>
        /// <see langword="false"/>: Item could not fully be added to inventory.
        /// </returns>
        /// <exception cref="InvalidInventoryOperationException"/>
        /// <exception cref="ItemNotFoundException"/>
        public bool AddItem(ItemCountTuple itemCountTuple)
        {
            // could not add full amount of items.
            if (GetSpaceRemaining(itemCountTuple.Item) < itemCountTuple.Count)
                return false;

            // find valid slots.
            (int[] matchingIndices, int[] emptyIndices) = FindValidSlotIndices(itemCountTuple.Item);

            // keep track of how many items are left.
            int amountRemaining = itemCountTuple.Count;

            foreach (int index in matchingIndices)
            {
                // items have been fully added.
                if (m_slots[index].AddToStack(itemCountTuple.Item, amountRemaining, out int remainder))
                {
                    return true;
                }
                // items could not be fully added.
                if (remainder > 0)
                    amountRemaining = remainder;
                // no remainder, meaning all items have been added.
                else
                    return true;
            }

            if (amountRemaining <= 0) return true;

            foreach (int index in emptyIndices)
            {
                // items have been fully added.
                if (m_slots[index].AddToStack(itemCountTuple.Item, amountRemaining, out int remainder))
                {
                    return true;
                }
                // items could not be fully added.
                if (remainder > 0)
                    amountRemaining = remainder;
                // no remainder, meaning all items have been added.
                else
                    return true;
            }

            return amountRemaining <= 0;
        }

        /// <summary>
        /// Removes a specified amount of an item from the inventory.
        /// </summary>
        /// <param name="itemCountTuple">The item and amount to remove.</param>
        /// <returns>
        /// <see langword="true"/>: All items were successfully removed.<br/>
        /// <see langword="false"/>: Not all items could be removed.
        /// </returns>
        public bool RemoveItem(ItemCountTuple itemCountTuple)
        {
            // items could not be fully added.
            if (GetItemTotal(itemCountTuple.Item) < itemCountTuple.Count)
                return false;

            Inventory preModifiedInventory = new(m_slots);

            int amountRemaining = itemCountTuple.Count;

            foreach (InventorySlot slot in m_slots)
            {
                // no item or unequivalent items.
                if (!slot.HasItem || slot.HasItem && slot.CurrentItem != itemCountTuple.Item) continue;

                // items were fully removed.
                if (slot.RemoveFromStack(itemCountTuple.Item, amountRemaining, out int remainder))
                    return true;

                // items were not fully removed.
                if (remainder > 0)
                    amountRemaining = remainder;
            }

            // all items were successfully removed.
            if (amountRemaining <= 0)
                return true;

            // items were not fully removed, reset ItemHolder.
            m_slots = preModifiedInventory.m_slots;
            return false;
        }

        /// <summary>
        /// Removes a collection of items from the inventory.
        /// </summary>
        /// <param name="itemCountTuples">The items to remove, and their corresponding amounts.</param>
        /// <returns>
        /// <see langword="true"/>: All items were successfully removed.<br/>
        /// <see langword="false"/>: Not all items could be removed.
        /// </returns>
        public bool RemoveItemRange(ItemCountTuple[] itemCountTuples)
        {
            foreach (ItemCountTuple itemTuple in itemCountTuples)
            {
                // an item could not be removed from the inventory.
                if (!RemoveItem(itemTuple)) return false;
            }

            // all items successfully removed.
            return true;
        }

        /// <summary>
        /// Checks for any slot containing a specified item.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>
        /// <see langword="true"/>: Item found.<br/>
        /// <see langword="false"/>: Item not found.
        /// </returns>
        public bool FindItem(ItemData item)
        {
            foreach (InventorySlot slot in m_slots)
            {
                // item found.
                if (slot.CurrentItem != item) continue;
                else return true;
            }
            // item not found.
            return false;
        }

        /// <summary>
        /// Finds a collection of items within the inventory.
        /// </summary>
        /// <param name="itemCountTuples">The items to search for, and their corresponding amounts.</param>
        /// <param name="remainingItems">The items that remain after the search.</param>
        /// <returns>
        /// <see langword="true"/>: All items were found within the inventory.<br/>
        /// <see langword="false"/>: Not all items could be found.
        /// </returns>
        public bool FindItemRange(ItemCountTuple[] itemCountTuples, ref int[] remainingItems)
        {
            // copy item counts to be modified, if this is the first iteration.
            if (remainingItems == null)
            {
                remainingItems = new int[itemCountTuples.Length];

                // populate copied array.
                for (int i = 0; i < itemCountTuples.Length; i++)
                {
                    remainingItems[i] = itemCountTuples[i].Count;
                }
            }

            for (int i = 0; i < itemCountTuples.Length; i++)
            {
                remainingItems[i] -= GetItemTotal(itemCountTuples[i].Item);
            }

            // if any item could not fully be satisfied, return false.
            foreach (int remainingItem in remainingItems)
                if (remainingItem > 0) return false;

            // all items satisfied.
            return true;
        }

        /// <summary>
        /// Finds the total amount of a specified item within the inventory.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>The total amount of the searched item within the inventory.</returns>
        /// <exception cref="InvalidInventoryOperationException"/>
        public int GetItemTotal(ItemData item)
        {
            if (item == null)
                throw new InvalidInventoryOperationException("Cannot get total of null item!");

            int total = 0;

            foreach (InventorySlot slot in m_slots)
            {
                if (slot.CurrentItem == null || slot.CurrentItem != item) continue;
                else
                    total += slot.CurrentStackSize;
            }

            return total;
        }

        /// <summary>
        /// Finds the total space available for a specified item.
        /// </summary>
        /// <param name="item">The item to search with.</param>
        /// <returns>The total slots that could be filled by the specified item.</returns>
        /// <exception cref="InvalidInventoryOperationException"/>
        public int GetSpaceRemaining(ItemData item)
        {
            if (item == null)
                throw new InvalidInventoryOperationException("Cannot iterate with null item!");

            int total = 0;

            foreach (InventorySlot slot in m_slots)
            {
                // slot empty, can fit full stack of item.
                if (!slot.HasItem)
                    total += item.MaxStackSize;
                // different items, cannot fit.
                else if (slot.CurrentItem != item)
                    continue;
                //same item, get remaining space in slot.
                else
                    total += item.MaxStackSize - slot.CurrentStackSize;
            }

            return total;
        }

        /// <summary>
        /// Finds all slots in the inventory that can hold a specified item.
        /// </summary>
        /// <param name="inventory">The inventory to search.</param>
        /// <param name="item">The item to search with.</param>
        /// <returns>An array of slots; first matching, then empty.</returns>
        private (int[] matchingIndices, int[] emptyIndices) FindValidSlotIndices(ItemData item)
        {
            if (item == null)
                throw new InvalidInventoryOperationException("Cannot iterate with null item!");

            List<int> matchingIndices = new();
            List<int> emptyIndices = new();

            for (int i = 0; i < m_slots.Length; i++)
            {
                if (!m_slots[i].HasItem)
                    emptyIndices.Add(i);
                else if (m_slots[i].HasItem && m_slots[i].CurrentItem == item)
                    matchingIndices.Add(i);
            }

            matchingIndices.AddRange(emptyIndices);
            return (matchingIndices.ToArray(), emptyIndices.ToArray());
        }
    }
}