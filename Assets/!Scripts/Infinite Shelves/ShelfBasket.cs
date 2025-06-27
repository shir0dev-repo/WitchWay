using UnityEngine;

/*
display ingrediant
change display if locked or unlocked
*/
[RequireComponent(typeof(Collider2D))]
public class ShelfBasket : MonoBehaviour
{
    [SerializeField] private IngredientSO storedIngredient;

    [Header("Mouse Detection")]
    [SerializeField] private Collider2D detectCollider;

    private bool inBounds = false;

    void Update()
    {
        CheckMouseInBounds();
    }

    private void CheckMouseInBounds()
    {
        if (detectCollider.bounds.Contains(GetMousePos()))
        {
            inBounds = true;
        }
        else
        {
            inBounds = false;
        }
    }

    private Vector2 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
