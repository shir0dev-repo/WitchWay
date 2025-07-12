using System.Collections;
using TMPro;
using UnityEngine;

public class StationsInvBoxItem : MonoBehaviour
{
    public ModifiedIngredient Ingredient => ingredient;
    [SerializeField] private ModifiedIngredient ingredient;
    [SerializeField] private StationsInvBox attachedBox;

    [SerializeField] private GameObject hoverUi;
    [SerializeField] private TMP_Text ingredientNameText;
    [SerializeField] private Collider2D detectCollider;

    private bool inBounds = false;
    private bool isHoverable = false;
    private bool canSpawn = false;

    void Start()
    {
        if (ingredient != null) ingredientNameText.text = ingredient.BaseIngredient.name;
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

    public void SetIngredient(ModifiedIngredient ingred)
    {
        ingredient = ingred;
    }

    public void SetAttchedBox(StationsInvBox box)
    {
        attachedBox = box;
    }

    public bool GetInBounds()
    {
        return inBounds;
    }

    private void SpawnWorldIngredient()
    {
        GameObject ing = Instantiate(ingredient.GetWorldRepresentation());
        WorldIngredient wIng = ing.GetComponent<WorldIngredient>();
        wIng.UpdateModifiers(ingredient);
        //WorldIngredient wIngred = Instantiate(ingredient.WorldPrefab, GetMousePos(), Quaternion.identity).GetComponent<WorldIngredient>();
        if (CursorManager.Instance != null)
            CursorManager.Instance.AttachToCursor(wIng, transform);

        attachedBox.GetVisuals().Remove(gameObject);
        attachedBox.RemoveItem(ingredient);

        attachedBox.OpenIngredients();

        if (TooltipCursor.Instance != null)
            TooltipCursor.Instance.OnUIItemUnhovered();

        Destroy(gameObject);
    }

    public void ToggleHoverable()
    {
        isHoverable = !isHoverable;

        StartCoroutine(WaitToSpawnable());
    }

    //helper
    private Vector2 GetMousePos()
    {
        Plane plane = new Plane(Vector3.forward, new Vector3 (0, 0, transform.position.z));
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

    private IEnumerator WaitToSpawnable()
    {
        yield return new WaitForSeconds(0.5f);

        canSpawn = true;
    }
}
