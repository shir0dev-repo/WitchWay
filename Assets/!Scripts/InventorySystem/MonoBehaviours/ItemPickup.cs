using UnityEngine;

namespace Shir0.InventorySystem
{
    /// <summary>
    /// Collidable object holding an amount of <see cref="ItemData"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CircleCollider2D))]
    [AddComponentMenu(menuName: "Inventory/Item Pickup")]
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private ItemCountTuple m_item;
        [SerializeField] private float m_pickupCooldown = 1.5f;

        bool m_canPickup = false;

        /// <summary>
        /// Assigns an amount of item to the pickup.
        /// </summary>
        /// <param name="itemTuple">The item to assign and its corresponding amount.</param>
        public void AssignItem(ItemCountTuple itemTuple)
        {
            m_item = itemTuple;
        }

        private void Update()
        {
            if (!m_canPickup)
            {
                m_pickupCooldown -= Time.deltaTime;
                if (m_pickupCooldown <= 0)
                    m_canPickup = true;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out ItemHolder holder))
                if (holder.AddItem(m_item))
                    Destroy(gameObject);
        }
    }
}