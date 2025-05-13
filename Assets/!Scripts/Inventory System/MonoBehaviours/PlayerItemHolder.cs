using UnityEngine;

namespace Shir0.InventorySystem
{
    /// <summary>
    /// Main item holder for the player.
    /// </summary>
    [AddComponentMenu(menuName: "Inventory/Player Item Holder")]
    public class PlayerItemHolder : ItemHolder
    {
        [SerializeField] private PlayerHotbar m_hotbar;

        /// <inheritdoc cref="ItemHolder.Awake"/>
        /// <remarks>Attempts to get a reference to the hotbar inventory.</remarks>
        protected override void Awake()
        {
            base.Awake();
            m_hotbar = GetComponent<PlayerHotbar>();
        }

        /// <inheritdoc cref="ItemHolder.AddItem(ItemCountTuple)"/>
        /// <remarks>Prioritizes existing stacks, then hotbar, then main inventory.</remarks>
        public override bool AddItem(ItemCountTuple itemTuple)
        {
            // check if slot in main inventory exists for item.
            if (m_inventory.FindItem(itemTuple.Item) && m_inventory.AddItem(itemTuple))
            {
                RequestInventoryRefresh();
                return true;
            }
            // check if hotbar exists and can add item.
            else if (m_hotbar != null && m_hotbar.Inventory.AddItem(itemTuple))
            {
                m_hotbar.RequestInventoryRefresh();
                return true;
            }
            // if existing stack does not exist, and hotbar cannot add item, try adding to inventory in empty slot.
            else if (m_inventory.AddItem(itemTuple))
            {
                RequestInventoryRefresh();
                return true;
            }
            // could not add item.
            else 
                return false;
        }

        /// <inheritdoc cref="ItemHolder.RemoveItem(ItemCountTuple)"/>
        /// <remarks>Prioritizes hotbar inventory if possible.</remarks>
        public override bool RemoveItem(ItemCountTuple itemCountTuple)
        {
            if (m_hotbar != null && m_hotbar.Inventory.RemoveItem(itemCountTuple))
                return true;

            else if (m_inventory.RemoveItem(itemCountTuple))
                return true;
            return false;
        }

        /// <inheritdoc cref="ItemHolder.RemoveItemRange(ItemCountTuple[])"/>
        /// <remarks>Checks both hotbar and main inventories to remove items.</remarks>
        public override bool RemoveItemRange(ItemCountTuple[] itemCountTuples)
        {
            // not all items could be found within both inventories.
            if (!FindItemRange(itemCountTuples))
                return false;

            // attempt to remove all items from both inventories.
            foreach (ItemCountTuple itemTuple in itemCountTuples)
            {
                // item could not be removed from either inventory.
                if (!RemoveItem(itemTuple)) return false;
            }

            // all items successfully removed.
            return true;
        }

        /// <inheritdoc cref="FindItemRange(ItemCountTuple[])"/>
        /// <remarks>Checks both hotbar and main inventories.</remarks>
        public override bool FindItemRange(ItemCountTuple[] itemCountTuples)
        {
            if (m_hotbar == null)
                return base.FindItemRange(itemCountTuples);

            int[] remainingItems = null;

            // iterate through main inventory, finding all items.
            bool withinInventory = m_inventory.FindItemRange(itemCountTuples, ref remainingItems);

            // items fully found within main inventory.
            if (withinInventory)
                return true;

            // iterate through hotbar, finding remaining items.
            bool withinHotbar = m_hotbar.Inventory.FindItemRange(itemCountTuples, ref remainingItems);

            // items not found within both inventories.
            if (!withinInventory && !withinHotbar)
                return false;
            // all items found.
            else return true;
        }

        /// <inheritdoc cref="ItemHolder.RequestInventoryRefresh"/>
        /// <remarks>Also refreshes hotbar, if possible.</remarks>
        public override void RequestInventoryRefresh()
        {
            base.RequestInventoryRefresh();
            if (m_hotbar != null)
                m_hotbar.RequestInventoryRefresh();
        }
    }
}