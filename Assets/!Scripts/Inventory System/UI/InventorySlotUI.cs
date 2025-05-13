using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Shir0.InventorySystem.UI
{
    /// <summary>
    /// User Interface for displaying it's corresponding <see cref="InventorySlot"/>.
    /// </summary>
    public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [System.Flags]
        private enum InteractionType
        {
            None = 0,
            HoverExit = 1,
            HoverEnter = 2,
            Click = 4
        }

        /// <summary>
        /// Invoked on hover enter and exit, or mouseclick.
        /// </summary>
        public static event UIInteractionHandler OnUISlotClicked;

        /// <summary>
        /// Text component displaying current stack size.
        /// </summary>
        [SerializeField] private TMPro.TextMeshProUGUI m_stackSize;

        /// <summary>
        /// Image displaying current item's Sprite.
        /// </summary>
        [SerializeField] private Image m_icon;

        [SerializeField] private InteractionType m_allowedInteractions;

        /// <summary>
        /// The internal slot assigned to this UI slot.
        /// </summary>
        private InventorySlot m_assignedSlot;

        /// <summary>
        /// The internal slot assigned to this UI slot.
        /// </summary>
        public InventorySlot AssignedSlot => m_assignedSlot;

        /// <summary>
        /// Initializes the UI slot by linking it to the supplied inventory slot.
        /// </summary>
        /// <param name="assignedSlot">The slot to link.</param>
        public void InitSlot(InventorySlot assignedSlot)
        {
            m_assignedSlot = assignedSlot;
            UpdateSlot();
        }

        /// <summary>
        /// Reflects any past changes to the assigned slot.
        /// </summary>
        public void UpdateSlot()
        {
            // reflect null slot reference.
            if (m_assignedSlot == null)
            {
                Debug.LogWarning($"{gameObject.name} has no assigned InventorySlot!");
                m_icon.sprite = null;
                m_icon.color = Color.clear;
                m_stackSize.text = "";
            }
            // reflect emptied slot.
            else if (m_assignedSlot.CurrentItem == null)
            {
                m_icon.sprite = null;
                m_icon.color = Color.clear;
                m_stackSize.text = "";
            }
            // reflect single item.
            else if (m_assignedSlot.CurrentStackSize == 1)
            {
                m_icon.sprite = m_assignedSlot.CurrentItem.Sprite;
                m_icon.color = Color.white;
                m_stackSize.text = "";
            }
            // reflect stacked item.
            else
            {
                m_icon.sprite = m_assignedSlot.CurrentItem.Sprite;
                m_icon.color = Color.white;
                m_stackSize.text = m_assignedSlot.CurrentStackSize.ToString();
            }
        }

        /// <summary>
        /// Called when pointer enters the UI slot.
        /// </summary>
        /// <param name="eventData">Provided by Unity's EventSystem.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_allowedInteractions.HasFlag(InteractionType.HoverEnter))
                OnUISlotClicked?.Invoke(new() { Sender = this, EventData = eventData });
        }

        /// <summary>
        /// Called when pointer click's the UI slot.
        /// </summary>
        /// <param name="eventData">Provided by Unity's EventSystem.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_allowedInteractions.HasFlag(InteractionType.Click))
                OnUISlotClicked?.Invoke(new() { Sender = this, EventData = eventData });
        }

        /// <summary>
        /// Called when pointer exits the UI slot.
        /// </summary>
        /// <param name="eventData">Provided by Unity's EventSystem.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (m_allowedInteractions.HasFlag(InteractionType.HoverExit)) 
                OnUISlotClicked?.Invoke(new() { Sender = this, EventData = eventData });
        }
    }
}