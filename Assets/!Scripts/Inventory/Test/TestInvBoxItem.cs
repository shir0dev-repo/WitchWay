using System.Collections;
using TMPro;
using UnityEngine;

public class TestInvBoxItem : MonoBehaviour
{
    public IngredientSO ingredient;
    public TestInvBox attachedBox;

    [SerializeField] private GameObject hoverUi;
    [SerializeField] private TMP_Text ingredientNameText;
    [SerializeField] private Collider2D detectCollider;

    public bool inBounds = false;
    private bool isHoverable = false;
    private bool canSpawn = false;

    void Start()
    {
        if (ingredient != null) ingredientNameText.text = ingredient.name;
    }

    void Update()
    {
        inBounds = CheckBounds2D(detectCollider.bounds, GetMousePos());

        if (inBounds)
        {
            if (Input.GetMouseButtonDown(0) && canSpawn)
            {
                SpawnWorldIngredient();
            }
            else if (isHoverable)
            {
                hoverUi.SetActive(true);
            }
        }
        else
        {
            hoverUi.SetActive(false);
        }
    }

    private void SpawnWorldIngredient()
    {
        WorldIngredient wIngred = Instantiate(ingredient.WorldPrefab, GetMousePos(), Quaternion.identity).GetComponent<WorldIngredient>();
        if (CursorManager.Instance != null)
            CursorManager.Instance.AttachToCursor(wIngred, transform);

        attachedBox.visuals.Remove(gameObject);
        attachedBox.RemoveItem(ingredient);

        attachedBox.GetComponent<TestInvBox>().OpenIngredients();

        Destroy(gameObject);
    }

    public void ToggleHoverable()
    {
        isHoverable = !isHoverable;

        StartCoroutine(WaitToSpawnable());
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

    private IEnumerator WaitToSpawnable()
    {
        yield return new WaitForSeconds(0.5f);

        canSpawn = true;
    }
}
