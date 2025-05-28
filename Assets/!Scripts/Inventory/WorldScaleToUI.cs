using UnityEngine;

public class WorldScaleToUI : MonoBehaviour
{
    [SerializeField] private RectTransform uiElement;
    [SerializeField] private BoxCollider myCollider;

    void Start()
    {
        Vector3[] worldCorners = new Vector3[4];
        uiElement.GetWorldCorners(worldCorners);

        Vector3 bottomLeft = worldCorners[0];
        Vector3 topRight = worldCorners[2];

        Vector3 center = (bottomLeft + topRight) / 2f;
        Vector2 size = topRight - bottomLeft;

        myCollider.transform.position = center;
        myCollider.size = new Vector3(size.x, size.y, myCollider.size.z);
    }
}
