namespace Shir0.InventorySystem
{
    /// <summary>
    /// Exception for a <see langword="null"/> or missing <see cref="Inventory"/>.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <item>
    /// <term>Use when:</term>
    /// <description>The <see cref="Inventory"/> or its <see cref="InventorySlot"/>[] may be <see langword="null"/>.</description>
    /// </item>
    /// </list>
    /// <example><b>Example:</b><code>
    ///     <see langword="if"/> (<paramref name="inventory"/> == <see langword="null"/>)<br/>
    ///         <see langword="throw new"/> <see cref="InventoryNotFoundException"/>("ItemHolder cannot be null!");
    ///     <see langword="else if"/> (<paramref name="inventory"/>.Slots == <see langword="null"/>)<br/>
    ///         <see langword="throw new"/> <see cref="InventoryNotFoundException"/>("ItemHolder slots cannot be null!");
    /// </code></example>
    /// </remarks>
    public class InventoryNotFoundException : System.Exception
    {
        public InventoryNotFoundException() { }
        public InventoryNotFoundException(string message) : base(message) { }
        public InventoryNotFoundException(string message, System.Exception inner) : base(message, inner) { }
    }
}