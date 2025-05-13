namespace Shir0.InventorySystem
{
    /// <summary>
    /// A linked item and its stack amount.
    /// </summary>
    [System.Serializable]
    public struct ItemCountTuple
    {
        /// <summary>
        /// The tuple's Item.
        /// </summary>
        public ItemData Item;
        /// <summary>
        /// The tuple's stack amount.
        /// </summary>
        public int Count;

        public ItemCountTuple(ItemData item, int count)
        {
            if (item == null)
                throw new ItemNotFoundException("Cannot create tuple with null item!");
            else if (count < 1)
                throw new InvalidInventoryOperationException("Cannot create tuple with less than one item!");

            Item = item;
            Count = count;
        }

        public readonly override string ToString()
        {
            return $"{Item.Name}: {Count}";
        }
    }
}