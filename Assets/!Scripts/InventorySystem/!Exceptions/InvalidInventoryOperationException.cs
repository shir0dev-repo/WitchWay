namespace Shir0.InventorySystem
{
    /// <summary>
    /// Exception for invalid operations performed on an <see cref="Inventory"/>.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <item>
    /// <term>Use when:</term>
    /// <description>An invalid <see cref="Inventory"/> operation is performed.</description>
    /// </item>
    /// </list>
    /// <example><b>Example:</b><code>
    /// <see langword="public"/> <see cref="Inventory"/>(<see langword="int"/> <paramref name="size"/>)<br/>&#123;<br/>
    ///     <see langword="if"/> (<paramref name="size"/> &lt; 1)
    ///         <see langword="throw new"/> <see cref="InvalidInventoryOperationException"/>("ItemHolder cannot be size zero!");<br/>
    ///     <see langword="else"/><br/>
    ///         <see cref="InventorySlot"/>[] m_slots = <see langword="new"/> <see cref="InventorySlot"/>[<paramref name="size"/>];<br/>
    /// &#125;
    /// </code></example>
    /// </remarks>
    public class InvalidInventoryOperationException : System.Exception
    {
        public InvalidInventoryOperationException() { }
        public InvalidInventoryOperationException(string message) : base(message) { }
        public InvalidInventoryOperationException(string message, System.Exception inner) : base(message, inner) { }
    }
}