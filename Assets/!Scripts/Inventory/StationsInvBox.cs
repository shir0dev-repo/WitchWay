using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationsInvBox : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Collider2D detectCollider;

    [Header("Prefabs")]
    [SerializeField] private GameObject boxItemPrefab;

    [Header("Display Settings")]
    [SerializeField] private float xMin = -1.25f;
    [SerializeField] private float xMax = 1.25f;
    [SerializeField] private float zMin = -0.2f;
    [SerializeField] private float zMax = 0.2f;

    private List<StationsInvBox> otherBoxes;
    private List<BasketItems> basketItems = new List<BasketItems>();
    private List<GameObject> visuals = new List<GameObject>();

    private bool inBounds = false;
    private bool clicked = false;

    void Start()
    {
        otherBoxes = new List<StationsInvBox>(FindObjectsByType<StationsInvBox>(FindObjectsSortMode.None));
        otherBoxes.Remove(this);

        SetupDisplay();
    }

    void Update()
    {
        CheckMouseInBounds();
    }

    public void SetupDisplay()
    {
        //clear vis
        foreach (GameObject obj in visuals)
        {
            Destroy(obj);
        }
        visuals.Clear();

        //display vis
        int index = 0;
        foreach (BasketItems item in basketItems)
        {
            for (int i = 0; i < item.itemAmount; i++)
            {
                float offsetX = Random.Range(xMin, xMax);
                float offsetZ = Random.Range(zMin, zMax);
                Vector3 offset = new Vector3(offsetX, 0.5f, offsetZ);
                Vector3 spawnPos = transform.position + offset;

                GameObject visual = Instantiate(boxItemPrefab, spawnPos, Quaternion.identity, transform);
                visual.name = "IngredientVisual_" + index++;

                StationsInvBoxItem boxItem = visual.GetComponent<StationsInvBoxItem>();
                if (boxItem != null)
                {
                    boxItem.SetIngredient(item.assignedIngredient);
                    boxItem.SetAttchedBox(this);
                }

                SpriteRenderer sr = visual.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingOrder = Mathf.RoundToInt(-offsetZ * 100);
                    sr.sprite = item.assignedIngredient.BaseIngredient.Sprite;
                    sr.transform.localScale = new Vector3(4, 4, 1);

                    BoxCollider2D collider = visual.GetComponent<BoxCollider2D>();
                    collider.size = new Vector2(sr.transform.localScale.x / 6, sr.transform.localScale.y / 6);
                }

                visuals.Add(visual);
            }
        }
    }

    private void CheckMouseInBounds()
    {
        inBounds = CheckBounds2D(detectCollider.bounds, GetMousePos());

        if (inBounds)
        {
            if (Input.GetMouseButtonDown(0))
            {
                foreach (StationsInvBox box in otherBoxes)
                {
                    if (box.clicked)
                    {
                        box.clicked = false;
                        box.CloseIngredients();
                        break;
                    }
                }

                OpenIngredients();
                clicked = true;
            }

            WorldIngredient ingredient;
            if (Input.GetMouseButtonUp(0) && CheckObjectsInBounds(out ingredient))
            {
                AddWorldItem(ingredient.ModifiedState);
                Destroy(ingredient.transform.gameObject);
                SetupDisplay();
            }
        }
        else if (!inBounds && clicked && Input.GetMouseButtonDown(0))
        {
            if (CheckIngredientDisplayBounds() == false)
            {
                clicked = false;
                CloseIngredients();
            }
        }
    }

    public void AddItem(BasketItems item)
    {
        basketItems.Add(item);
    }

    public void AddWorldItem(ModifiedIngredient ingredient)
    {
        basketItems.Add(new BasketItems(transform, ingredient, 1));
    }

    public void RemoveItem(ModifiedIngredient ingredient)
    {
        foreach (BasketItems item in basketItems)
        {
            if (item.assignedIngredient == ingredient)
            {
                if (item.itemAmount > 1) item.itemAmount -= 1;
                else
                {
                    basketItems.Remove(item);
                }

                break;
            }
        }
    }

    public void ClearItems()
    {
        basketItems.Clear();
    }

    public void OpenIngredients()
    {
        print(gameObject.name + " opened");

        int itemsPerRow = 5;
        float spacing = 0.8f;
        float verticalOffset = 1.4f;

        int itemCount = visuals.Count;

        for (int i = 0; i < itemCount; i++)
        {
            int row = i / itemsPerRow;
            int col = i % itemsPerRow;

            int itemsInThisRow = Mathf.Min(itemsPerRow, itemCount - row * itemsPerRow);
            float rowWidth = (itemsInThisRow - 1) * spacing;
            float offsetX = -rowWidth / 2f + col * spacing;
            float offsetY = row * spacing;

            Vector3 localOffset = new Vector3(offsetX, verticalOffset + offsetY, 0f);
            visuals[i].transform.position = transform.position + localOffset;

            SpriteRenderer sr = visuals[i].GetComponentInChildren<SpriteRenderer>();
            sr.sortingOrder = 100 + row;

            visuals[i].GetComponent<StationsInvBoxItem>().ToggleHoverable();
        }
    }

    public void CloseIngredients()
    {
        print(gameObject.name + " closed");
        SetupDisplay(); //temp! destorying and recreaing lots of objects is bad practice
    }

    private bool CheckIngredientDisplayBounds()
    {
        foreach (GameObject boxItem in visuals)
        {
            if (boxItem.GetComponent<StationsInvBoxItem>().GetInBounds() == true)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckObjectsInBounds(out WorldIngredient ingredient)
    {
        ingredient = null;
        foreach (WorldIngredient ingred in FindObjectsByType<WorldIngredient>(FindObjectsSortMode.None))
        {
            if (CheckBounds2D(detectCollider.bounds, ingred.transform.position) && ingred.transform.position.z == transform.position.z)
            {
                ingredient = ingred;
                return true;
            }
        }

        return false;
    }

    public List<GameObject> GetVisuals()
    {
        return visuals;
    }

    //helpers
    private Vector2 GetMousePos()
    {
        Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, transform.position.z));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            return new Vector2(worldPoint.x, worldPoint.y);
        }

        return Vector2.zero;
    }

    private bool CheckBounds2D(Bounds bounds, Vector2 position)
    {
        if (position.x >= bounds.min.x && position.x <= bounds.max.x &&
        position.y >= bounds.min.y && position.y <= bounds.max.y)
        {
            return true;
        }

        return false;
    }
}
