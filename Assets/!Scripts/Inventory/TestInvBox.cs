using UnityEngine;

public class TestInvBox : MonoBehaviour
{
    [SerializeField] private Collider2D detectCollider;

    private bool inBounds = false;

    public bool clicked;

    void Update()
    {
        CheckMouseInBounds();
    }

    private void CheckMouseInBounds()
    {
        if (detectCollider.bounds.Contains(GetMousePos()))
        {
            if (CheckBounds2D(detectCollider.bounds, GetMousePos()))
            {
                inBounds = true;
            }
        }

        if (inBounds)
        {
            if (Input.GetMouseButtonDown(0))
            {
                print("clicked " + gameObject.name);
                clicked = true;
            }
        }
        else if (!inBounds && clicked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //explode
            }
        }
    }

    public void OpenIngredients()
    {
        print("open");
    }

    public void CloseIngredients()
    {
        print("close");
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
