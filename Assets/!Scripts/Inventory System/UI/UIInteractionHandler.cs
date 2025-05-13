using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Shir0.InventorySystem.UI
{
    public delegate void UIInteractionHandler(UISlotInteractionArgs args);

    /// <summary>
    /// An <see langword="event"/> argument for <see cref="UIInteractionHandler"/>.
    /// </summary>
    public class UISlotInteractionArgs
    {
        public InventorySlotUI Sender;
        /// <summary>
        /// The type of interaction performed with this UI slot.
        /// </summary>
        public PointerEventData EventData;

        public UISlotInteractionArgs() { }
    }
}