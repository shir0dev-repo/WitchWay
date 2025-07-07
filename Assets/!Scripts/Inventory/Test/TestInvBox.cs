using System.Collections.Generic;
using UnityEngine;

public class TestInvBox : MonoBehaviour
{
    [SerializeField] private Collider2D detectCollider;

    private List<TestInvBox> others;
    private List<BasketItems> basketItems = new List<BasketItems>();
    private List<StationsDisplayIngredient> visuals = new List<StationsDisplayIngredient>();

    private bool inBounds = false;

    public bool clicked;

    void Start()
    {
        others = new List<TestInvBox>(FindObjectsByType<TestInvBox>(FindObjectsSortMode.None));
        others.Remove(this);
        SetupDisplay();
    }

    void Update()
    {
        CheckMouseInBounds();
    }

    private void SetupDisplay()
    {
        //clear vis
        foreach (var v in visuals)
        {
            if (v.visualObject != null)
                Destroy(v.visualObject);
        }
        visuals.Clear();

        //make vis
        foreach (BasketItems item in basketItems)
        {
            for (int i = 0; i < item.itemAmount; i++)
            {
                visuals.Add(new StationsDisplayIngredient(item.assignedIngredient));
            }
        }

        //display vis
        float xMin = -1.25f, xMax = 1.25f;
        float zMin = -0.2f, zMax = 0.2f;
        int itemCount = visuals.Count;

        for (int i = 0; i < itemCount; i++)
        {
            float offsetX = Random.Range(xMin, xMax);
            float offsetZ = Random.Range(zMin, zMax);

            Vector3 localOffset = new Vector3(offsetX, 0f, offsetZ);
            Vector3 worldPos = transform.position + localOffset;

            GameObject visual = new GameObject("IngredientVisual_" + i);
            visual.transform.position = new Vector3(worldPos.x, 0.5f, worldPos.z);
            visual.transform.parent = transform;
            visual.transform.localScale = new Vector3(4, 4, 1);

            SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
            sr.sprite = visuals[i].ingredient.Sprite;

            sr.sortingOrder = Mathf.RoundToInt(-offsetZ * 100);

            visuals[i].visualObject = visual;
        }
    }

    private void CheckMouseInBounds()
    {
        inBounds = CheckBounds2D(detectCollider.bounds, GetMousePos());

        if (inBounds)
        {
            if (Input.GetMouseButtonDown(0))
            {
                foreach (TestInvBox box in others)
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
        }
        else if (!inBounds && clicked)
        {
            
        }
    }

    public void AddItem(BasketItems item)
    {
        basketItems.Add(item);
    }

    private void RemoveItem()
    {

    }

    public void ClearItems()
    {
        basketItems.Clear();
    }

    public void OpenIngredients()
    {
        print(gameObject.name + " opened");

        int itemsPerRow = 5;
        float spacing = 0.5f;
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
            visuals[i].visualObject.transform.position = transform.position + localOffset;

            SpriteRenderer sr = visuals[i].visualObject.GetComponent<SpriteRenderer>();
            sr.sortingOrder = 100 + row;
        }
    }

    public void CloseIngredients()
    {
        print(gameObject.name + " closed");
        SetupDisplay(); //temp! destorying and recreaing lots of objects is bad practice
    }

    private Vector2 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(mousePos);
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
