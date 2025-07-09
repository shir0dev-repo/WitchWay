using UnityEngine;

public class TestInvBoxItem : MonoBehaviour
{
    public IngredientSO ingredient;
    public GameObject attachedBox;

    [SerializeField] private GameObject hoverUi;
    [SerializeField] private Collider2D detectCollider;

    private bool inBounds = false;
    private bool isHoverable = false;

    void Update()
    {
        inBounds = CheckBounds2D(detectCollider.bounds, GetMousePos());

        if (inBounds)
        {
            if (Input.GetMouseButtonDown(0))
            {

            }
            else if(isHoverable)
            {
                hoverUi.SetActive(true);
            }
        }
        else
        {
            hoverUi.SetActive(false);
        }
    }

    public void ToggleHoverable()
    {
        isHoverable = !isHoverable;
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
