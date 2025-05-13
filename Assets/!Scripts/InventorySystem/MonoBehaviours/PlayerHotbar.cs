using UnityEngine;

namespace Shir0.InventorySystem
{
    /// <summary>
    /// Additional item holder for the player, with ability to select item's within the inventory and use them.
    /// </summary>
    [AddComponentMenu(menuName: "Inventory/Player Hotbar")]
    public class PlayerHotbar : ItemHolder
    {
        private PlayerItemHolder m_mainInventory;

        /// <summary>
        /// A reference to the players main inventory.
        /// </summary>
        /// <remarks>Can be null.</remarks>
        public PlayerItemHolder MainInventory => m_mainInventory;

        /// <inheritdoc cref="ItemHolder.Awake"/>
        /// <remarks>Attempts to get a reference to the main inventory.</remarks>
        protected override void Awake()
        {
            base.Awake();
            m_mainInventory = GetComponent<PlayerItemHolder>();
        }

        /// <inheritdoc cref="ItemHolder.AddItem(ItemCountTuple)"/>
        /// <remarks>Prioritizes main inventory if possible.</remarks>
        public override bool AddItem(ItemCountTuple itemCountTuple)
        {
            if (m_mainInventory != null)
                return m_mainInventory.AddItem(itemCountTuple);
            else
                return m_inventory.AddItem(itemCountTuple);
        }

        /// <inheritdoc cref="ItemHolder.RemoveItem(ItemCountTuple)"/>
        /// <remarks>Prioritizes this inventory, then the main inventory if possible.</remarks>
        public override bool RemoveItem(ItemCountTuple itemCountTuple)
        {
            if (m_inventory.RemoveItem(itemCountTuple))
                return true;
            else if (m_mainInventory != null && m_mainInventory.RemoveItem(itemCountTuple))
                return true;
            else
                return false;
        }

        /// <inheritdoc cref="RemoveItemRange(ItemCountTuple[])"/>
        /// <remarks>Prioritizes main inventory if possible.</remarks>
        public override bool RemoveItemRange(ItemCountTuple[] itemCountTuples)
        {
            if (m_mainInventory != null)
                return m_mainInventory.RemoveItemRange(itemCountTuples);
            else 
                return base.RemoveItemRange(itemCountTuples);
        }

        /// <inheritdoc cref="FindItemRange(ItemCountTuple[])"/>
        /// <remarks>Prioritizes main inventory if possible.</remarks>
        public override bool FindItemRange(ItemCountTuple[] itemCountTuples)
        {
            if (m_mainInventory != null)
                return m_mainInventory.FindItemRange(itemCountTuples);
            else 
                return base.FindItemRange(itemCountTuples);
        }
    }
}