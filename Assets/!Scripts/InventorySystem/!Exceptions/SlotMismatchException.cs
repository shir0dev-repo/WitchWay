namespace Shir0.InventorySystem
{
    /// <summary>
    /// Exception for when <see cref="InventorySlot"/>[] and <see cref="InventorySlotUI"/>[] have differing sizes.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <item>
    /// <term>Use when:</term>
    /// <description>Refering to an <see cref="InventorySlot"/>[] and its <see cref="InventorySlotUI"/>[] counterpart.</description>
    /// </item>
    /// <item><term>Example:</term></item>
    /// </list>
    /// <code>
    ///     <see langword="if"/> (<paramref name="m_inventorySlots"/>.Length != <paramref name="m_uiSlots"/>.Length)
    ///         <see langword="throw new"/> <see cref="SlotMismatchException"/>("Slot arrays cannot be of differing sizes!");
    /// </code>
    /// </remarks>
    public class SlotMismatchException : System.Exception
    {
        public SlotMismatchException() { }
        public SlotMismatchException(string message) : base(message) { }
        public SlotMismatchException(string message, System.Exception inner) : base(message, inner) { }
    }
}