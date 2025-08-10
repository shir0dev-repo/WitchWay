using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotScript : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private IngredientSO ingredient;
    private InventoryGridView grid;
    private InventoryHolder holder;
    private InventorySlotData mySlot;

    private bool inWz = false;
    private bool pointerDownInBounds = false;

    void Start()
    {
        holder = GameObject.FindFirstObjectByType<InventoryHolder>();
        grid = GetComponentInParent<InventoryGridView>(true);
    }

    public void SetIngredient(IngredientSO newIngred)
    {
        ingredient = newIngred;
    }

    public void BindSlot(InventorySlotData slot)
    {
        mySlot = slot;
    }

    public void ActiveInWZ()
    {
        inWz = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownInBounds = false;
        if (holder != null && holder.InvBounds != null)
        {
            pointerDownInBounds = RectTransformUtility.RectangleContainsScreenPoint(holder.InvBounds, eventData.position, eventData.pressEventCamera);
        }

        /*if (ingredient != null)
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
        }*/
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!pointerDownInBounds || grid == null || mySlot == null) return;

        grid.BeginDrag(mySlot, gameObject, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (grid == null) return;

        grid.Drag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (grid == null || holder == null || holder.InvBounds == null) return;

        bool inside = RectTransformUtility.RectangleContainsScreenPoint(holder.InvBounds, eventData.position, eventData.pressEventCamera);

        if (!inside)
        {
            SpawnWorldIngredient();
            grid.EndDrag(true);
            holder.RemoveItem(ingredient);
        }
        else
        {
            grid.EndDrag(false);
        }

        pointerDownInBounds = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!inWz) return;

        //display tooltip
        print("entered");
    }

    private Vector2 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void SpawnWorldIngredient()
    {
        if (inWz)
        {
            float forwardDist = 5f;

            Vector3 spawnPos = FindFirstObjectByType<WZPlayerInteract>().transform.position + FindFirstObjectByType<WZPlayerInteract>().transform.forward * forwardDist;
            RaycastHit hit;
            if (Physics.Raycast(spawnPos, Vector3.down, out hit, Mathf.Infinity))
            {
                spawnPos = hit.point;
            }

            GameObject worldObject = Instantiate(ingredient.WorldPrefab, spawnPos, Quaternion.identity);
            Destroy(worldObject.GetComponent<WorldIngredient>());
            Destroy(worldObject.GetComponent<CrushableIngredientState>());
            worldObject.GetComponent<WZWorldIngredient>().ingredient = ingredient;
            worldObject.transform.localScale = worldObject.transform.localScale * 0.25f; //might want to make configurable
        }
        else
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
        }
    }
}
    
