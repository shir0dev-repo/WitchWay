using UnityEngine;

namespace Shir0.InventorySystem
{
    /// <summary>
    /// abstract MonoBehaviour holding an <see cref="Inventory"/>.
    /// </summary>
    public abstract class ItemHolder : MonoBehaviour
    {
        [SerializeField] protected Inventory m_inventory;
        [SerializeField] protected int m_size;

        /// <summary>
        /// Requests the inventory's display to be updated.
        /// </summary>
        public System.Action OnInventoryDisplayRequested;

        /// <summary>
        /// The inventory assigned to this item holder.
        /// </summary>
        /// <exception cref="InventoryNotFoundException"/>
        public Inventory Inventory
        {
            get
            {
                if (m_inventory == null)
                    throw new InventoryNotFoundException();
                else
                    return m_inventory;
            }
        }

        /// <summary>
        /// The size of this item holder's inventory.
        /// </summary>
        public int Size => m_size;

        /// <summary>
        /// Initializes inventory.
        /// </summary>
        protected virtual void Awake()
        {
            m_inventory = new Inventory(m_size);
        }

        /// <summary>
        /// Adds a specified amount of item to the inventory.
        /// </summary>
        /// <param name="itemCountTuple">The item and amount to add.</param>
        /// <returns>
        /// <see langword="true"/>: All items were successfully added to the inventory.<br/>
        /// <see langword="false"/>: Not all items could be added to the inventory.
        /// </returns>
        public abstract bool AddItem(ItemCountTuple itemCountTuple);

        /// <summary>
        /// Removes a specified amount of item to the inventory.
        /// </summary>
        /// <param name="itemCountTuple">The item and amount to add.</param>
        /// <returns>
        /// <see langword="true"/>: All items were successfully removed from the inventory.<br/>
        /// <see langword="false"/>: Not all items could be removed from the inventory.
        /// </returns>
        public abstract bool RemoveItem(ItemCountTuple itemCountTuple);

        /// <summary>
        /// Removes a collection of items and their corresponding amounts.
        /// </summary>
        /// <param name="itemCountTuples">Collection of items to be removed.</param>
        /// <returns>
        /// <see langword="true"/>: All items were successfully removed from the inventory.<br/>
        /// <see langword="false"/>: Not all items could be removed from the inventory.
        /// </returns>
        public virtual bool RemoveItemRange(ItemCountTuple[] itemCountTuples)
        {
            return m_inventory.RemoveItemRange(itemCountTuples);
        }

        /// <summary>
        /// Compares a collection of items to see if the inventory fully contains them.
        /// </summary>
        /// <param name="itemCountTuples">Collection of items to search for.</param>
        /// <returns>
        /// <see langword="true"/>: All items were successfully found within the inventory.<br/>
        /// <see langword="false"/>: Not all items could be found within the inventory.
        /// </returns>
        public virtual bool FindItemRange(ItemCountTuple[] itemCountTuples)
        {
            int[] remainingItems = null;
            return m_inventory.FindItemRange(itemCountTuples, ref remainingItems);
        }

        /// <summary>
        /// Requests the inventory's display to be updated.
        /// </summary>
        public virtual void RequestInventoryRefresh() => OnInventoryDisplayRequested?.Invoke();
    }
}