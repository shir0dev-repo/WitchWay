namespace Shir0.InventorySystem
{
    /// <summary>
    /// Exception for a <see langword="null"/> <see cref="ItemData"/> being referenced.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <item>
    /// <term>Use when:</term>
    /// <description>The <see cref="ItemData"/> being referenced is <see langword="null"/>.</description>
    /// </item>
    /// <item><term>Example:</term></item>
    /// </list>
    /// <code>
    ///     <see langword="if"/> (<paramref name="item"/> == <see langword="null"/>)<br/>
    ///         <see langword="throw new"/> <see cref="ItemNotFoundException"/>("Item cannot be null!");
    /// </code>
    /// </remarks>
    public class ItemNotFoundException : System.Exception
    {
        public ItemNotFoundException() { }
        public ItemNotFoundException(string message) : base(message) { }
        public ItemNotFoundException(string message, System.Exception inner) : base(message, inner) { }
    }
}