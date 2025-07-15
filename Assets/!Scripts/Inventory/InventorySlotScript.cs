using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotScript : MonoBehaviour, IPointerDownHandler
{
    private Inventory inventory;
    private IngredientSO ingredient;

    void Start()
    {
        inventory = GameObject.FindFirstObjectByType<Inventory>();
    }

    public void SetIngredient(IngredientSO newIngred)
    {
        ingredient = newIngred;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (ingredient != null)
        {
            GameObject worldObject = Instantiate(ingredient.WorldPrefab, GetMousePos(), Quaternion.identity);
            WorldIngredient wIngred = worldObject.GetComponent<WorldIngredient>();
            if (wIngred)
            {
                if (worldObject.GetComponent<Rigidbody>())
                {
                    worldObject.GetComponent<Rigidbody>().isKinematic = false;
                    worldObject.GetComponent<Rigidbody>().useGravity = false;
                }

                if (CursorManager.Instance != null) CursorManager.Instance.AttachToCursor(wIngred, worldObject.transform);
            }

            inventory.RemoveItem(ingredient);
        }
    }

    private Vector2 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
