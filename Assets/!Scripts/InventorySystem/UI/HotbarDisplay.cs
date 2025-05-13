namespace Shir0.InventorySystem.UI
{
    public class HotbarDisplay : InventoryDisplay
    {
        private InventorySlotUI m_lastSelectedHotbarSlot;

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Start()
        {
            base.Start();
            SelectHotbarSlot(0);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        private void SelectHotbarSlot(int hotbarSlotID)
        {
            // dehighlight selected slot if possible.
            if (m_lastSelectedHotbarSlot != null)
                ToggleSlotHighlight(m_lastSelectedHotbarSlot, false);

            // assign selected slot.
            m_lastSelectedHotbarSlot = m_uiSlots[hotbarSlotID];

            // highlight reassigned slot.
            ToggleSlotHighlight(m_uiSlots[hotbarSlotID], true);

        }
    }
}