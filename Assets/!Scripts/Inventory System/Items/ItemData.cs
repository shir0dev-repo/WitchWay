using UnityEngine;

namespace Shir0.InventorySystem
{
    /// <summary>
    /// Base <see langword="class"/> for creating <see cref="Inventory"/> items, containing an ID, Name, and Description.
    /// </summary>
    /// <remarks>
    /// The <see cref="ItemData"/> <see cref="ScriptableObject"/> is the backbone of the <see cref="InventorySystem"/>.<br/>
    /// In the future, ideally this <see langword="class"/> will become <see langword="abstract"/>, only intended to be<br/>
    /// inherited from, as different <see cref="ItemData"/> will have different functionality.
    /// </remarks>
    [CreateAssetMenu(fileName = "New Item Data", menuName = "Inventory/New Item")]
    public class ItemData : SODatabase.DatabaseObject
    {
        /// <summary>
        /// A short text, describing the item.
        /// </summary>
        [TextArea] public string Description;

        /// <summary>
        /// The item's maximum stack size.
        /// </summary>
        [Min(1)] public int MaxStackSize = 99;

        /// <summary>
        /// The item's icon when inside the inventory.
        /// </summary>
        public Sprite Sprite;
    }
}