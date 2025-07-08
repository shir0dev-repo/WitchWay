using System.Collections.Generic;
using UnityEngine;

public class TestInvBox : MonoBehaviour
{
    [SerializeField] private Collider2D detectCollider;
    [SerializeField] private GameObject boxItemPrefab;

    private List<TestInvBox> others;
    private List<BasketItems> basketItems = new List<BasketItems>();
    private List<GameObject> visuals = new List<GameObject>();

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
        foreach (GameObject obj in visuals)
        {
            Destroy(obj);
        }
        visuals.Clear();

        //display vis
        float xMin = -1.25f, xMax = 1.25f;
        float zMin = -0.2f, zMax = 0.2f;

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

                TestInvBoxItem boxItem = visual.GetComponent<TestInvBoxItem>();
                if (boxItem != null)
                {
                    boxItem.ingredient = item.assignedIngredient;
                }

                SpriteRenderer sr = visual.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingOrder = Mathf.RoundToInt(-offsetZ * 100);
                    sr.sprite = item.assignedIngredient.Sprite;
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
            visuals[i].transform.position = transform.position + localOffset;

            SpriteRenderer sr = visuals[i].GetComponentInChildren<SpriteRenderer>();
            sr.sortingOrder = 100 + row;

            visuals[i].GetComponent<TestInvBoxItem>().ToggleHoverable();
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
