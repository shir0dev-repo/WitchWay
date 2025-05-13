namespace Shir0.InventorySystem
{
    /// <summary>
    /// Holds and manipulates an amount of <see cref="ItemData"/>.
    /// </summary>
    [System.Serializable]
    public class InventorySlot
    {
        [UnityEngine.SerializeField] private ItemData m_item;
        [UnityEngine.SerializeField] private int m_currentStackSize = -1;

        /// <summary>
        /// The current item this slot holds.
        /// </summary>
        public ItemData CurrentItem => m_item;

        /// <summary>
        /// The size of the current item's stack.
        /// </summary>
        public int CurrentStackSize => m_currentStackSize;

        /// <summary>
        /// If the slot's current item is not null.
        /// </summary>
        public bool HasItem => m_item != null;

        /// <summary>
        /// Creates an empty slot.
        /// </summary>
        public InventorySlot()
        {
            ClearSlot();
        }

        /// <summary>
        /// Creates a slot with a specified item and amount.
        /// </summary>
        /// <param name="itemTuple">The item stack to assign.</param>
        public InventorySlot(ItemCountTuple itemTuple)
        {
            AssignSlot(itemTuple);
        }

        /// <summary>
        /// Creates a copy of an existing slot.
        /// </summary>
        /// <param name="other">The slot to copy from.</param>
        public InventorySlot(InventorySlot other)
        {
            AssignSlot(other);
        }

        /// <summary>
        /// Assigns an item and stack size to the slot.
        /// </summary>
        /// <param name="item">Item to assign.</param>
        /// <param name="stackSize">Amount to assign.</param>
        public void AssignSlot(ItemData item, int stackSize)
        {
            if (stackSize < 1 || item == null)
                return;

            m_item = item;
            m_currentStackSize = stackSize;
        }

        /// <summary>
        /// Assigns an item stack to the slot.
        /// </summary>
        /// <param name="itemTuple">Item stack to assign.</param>
        public void AssignSlot(ItemCountTuple itemTuple)
        {
            if (itemTuple.Count < 1 || itemTuple.Item == null)
                return;

            m_item = itemTuple.Item;
            m_currentStackSize = itemTuple.Count;
        }
        /// <summary>
        /// Copies another slot's data onto this slot.
        /// </summary>
        /// <param name="other">The slot to copy from.</param>
        public void AssignSlot(InventorySlot other)
        {
            m_item = other.m_item;
            m_currentStackSize = other.m_currentStackSize;

            if (m_currentStackSize < 1 || m_item == null)
                ClearSlot();
        }

        /// <summary>
        /// Clears this slot's stack of items.
        /// </summary>
        public void ClearSlot()
        {
            m_item = null;
            m_currentStackSize = -1;
        }

        /// <summary>
        /// Adds to this slot.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="amount">Amount to add.</param>
        /// <param name="remainder">Amount remaining after adding to stack.</param>
        /// <returns>
        /// <see langword="true"/>: Amount was fully added to the slot.<br/>
        /// <see langword="false"/>: Amount could not be fully added to the slot.
        /// </returns>
        public bool AddToStack(ItemData item, int amount, out int remainder)
        {
            remainder = amount;

            // cannot add less than one or null item.
            if (amount < 1 || item == null)
                return false;

            // slot has no item.
            if (m_item == null)
            {
                // amount too large to fit in one stack.
                if (amount > item.MaxStackSize)
                {
                    remainder = amount - item.MaxStackSize;
                    AssignSlot(item, item.MaxStackSize);
                    return false;
                }
                // amount fits in one stack.
                else
                {
                    AssignSlot(item, amount);
                    remainder = 0;
                    return true;
                }
            }
            // slot has item.
            else
            {
                // different items or no space, can't add items.
                if (m_item != item || m_currentStackSize == m_item.MaxStackSize)
                {
                    remainder = amount;
                    return false;
                }
                // enough space, add amount to stack.
                else if (m_currentStackSize + amount <= m_item.MaxStackSize)
                {
                    m_currentStackSize += amount;
                    remainder = 0;
                    return true;
                }
                // not enough space, fill stack and assign remainder.
                else
                {
                    remainder = m_currentStackSize + amount - m_item.MaxStackSize;
                    m_currentStackSize = m_item.MaxStackSize;
                    return false;
                }
            }
        }

        /// <summary>
        /// Removes from this slot.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <param name="amount">Amount to remove.</param>
        /// <param name="remainder">Amount remaining after removing from stack.</param>
        /// <returns>
        /// <see langword="true"/>: Amount is fully removed from the slot.<br/>
        /// <see langword="false"/>: Amount could not be fully removed from the slot.
        /// </returns>
        public bool RemoveFromStack(ItemData item, int amount, out int remainder)
        {
            remainder = amount;

            // cannot remove less than one or null item.
            if (amount < 1 || item == null)
                return false;
            // this slot is empty.
            else if (m_item == null || m_currentStackSize < 1)
                return false;
            // differing items, cannot remove.
            else if (m_item != item)
                return false;

            // amount cannot be fully removed from this slot.
            if (m_currentStackSize - amount < 0)
            {
                remainder = amount - m_currentStackSize;
                ClearSlot();
                return false;
            }
            // amount can be fully removed from this slot.
            else
            {
                remainder = 0;
                m_currentStackSize -= amount;

                if (m_currentStackSize < 1 || m_item == null)
                    ClearSlot();

                return true;
            }
        }

        /// <summary>
        /// Splits the current stack in half.
        /// </summary>
        /// <returns>The slot created after splitting this slot.</returns>
        public InventorySlot SplitStack()
        {
            InventorySlot splitSlot = new InventorySlot(this);
            splitSlot.m_currentStackSize /= 2;

            // if stack size is one, dividing by 2 will truncate to zero, rendering this method useless. in such cases, set the stack size to one.
            if (splitSlot.m_currentStackSize < 1)
                splitSlot.m_currentStackSize = 1;

            m_currentStackSize -= splitSlot.m_currentStackSize;

            if (m_currentStackSize < 1 || m_item == null)
                ClearSlot();

            return splitSlot;
        }
    }
}