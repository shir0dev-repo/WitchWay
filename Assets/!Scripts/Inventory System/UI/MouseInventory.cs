using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Shir0.InventorySystem.UI
{
    public class MouseInventory : MonoBehaviour
    {
        [SerializeField] private Camera m_camera;
        [Space]
        [SerializeField] private InventorySlotUI m_mouseUISlot;
        [SerializeField] private InventorySlot m_mouseSlot;

        /// <summary>
        /// The internal slot held by the cursor.
        /// </summary>
        public InventorySlot Slot => m_mouseSlot;

        /// <summary>
        /// The UI slot shown on the cursor.
        /// </summary>
        public InventorySlotUI UISlot => m_mouseUISlot;

        private void Awake()
        {
            if (m_camera == null)
                m_camera = Camera.main;
            if (m_mouseUISlot == null)
                m_mouseUISlot = GetComponentInChildren<InventorySlotUI>();

            m_mouseUISlot.InitSlot(m_mouseSlot);
        }
        private void Update() => FollowCursor();

        /// <summary>
        /// Follows the cursor in world space, without changing z-position.
        /// </summary>
        private void FollowCursor()
        {
            Vector3 mousePos = InputSystem.actions.FindActionMap("UI").FindAction("Point", true).ReadValue<Vector2>();
            Vector2 pos = mousePos;
            transform.position = new(pos.x, pos.y, transform.position.z);
        }

        /// <summary>
        /// Updates the UI slot on the cursor.
        /// </summary>
        public void UpdateMouseUI()
        {
            m_mouseUISlot.UpdateSlot();
        }

        /// <summary>
        /// Assigns the inventory slot on the cursor.
        /// </summary>
        /// <param name="slot">Slot data to assign.</param>
        public void AssignMouseSlot(InventorySlot slot)
        {
            m_mouseSlot.AssignSlot(slot.CurrentItem, slot.CurrentStackSize);
        }

        /// <summary>
        /// Updates the inventory slot on the cursor.
        /// </summary>
        /// <param name="itemTuple">Amount of item to assign.</param>
        public void AssignMouseSlot(ItemCountTuple itemTuple)
        {
            m_mouseSlot.AssignSlot(itemTuple);
        }
    }
}