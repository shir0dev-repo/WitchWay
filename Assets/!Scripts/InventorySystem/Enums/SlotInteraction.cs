using UnityEngine.EventSystems;

namespace Shir0.InventorySystem
{
    /// <summary>
    /// Type of interaction performed on a slot.
    /// </summary>
    public enum SlotInteraction
    {
        HoverEnter,
        HoverExit,
        ClickLeft,
        ClickRight,
        ClickMiddle
    }

    /// <summary>
    /// Utility class for utilizing <see cref="SlotInteraction"/>.
    /// </summary>
    public static class SlotClickUtils
    {
        /// <summary>
        /// Converts <see cref="PointerEventData"/> to <see cref="SlotInteraction"/>.
        /// </summary>
        /// <remarks>If a mouse button not registered in this method is pressed, default to <see cref="SlotInteraction.ClickLeft"/>.</remarks>
        /// <param name="eventData">Obtained from <see cref="IPointerClickHandler.OnPointerClick(PointerEventData)"/>.</param>
        /// <returns>The <see cref="PointerEventData.InputButton"/> from <paramref name="eventData"/> as a <see cref="SlotInteraction"/>.</returns>
        public static SlotInteraction GetClickType(this PointerEventData eventData)
            => eventData.button switch
            {
                PointerEventData.InputButton.Left => SlotInteraction.ClickLeft,
                PointerEventData.InputButton.Right => SlotInteraction.ClickRight,
                PointerEventData.InputButton.Middle => SlotInteraction.ClickMiddle,
                _ => SlotInteraction.HoverExit
            };
    }
}