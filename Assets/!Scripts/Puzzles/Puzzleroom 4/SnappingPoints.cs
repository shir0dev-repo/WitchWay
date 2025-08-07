using UnityEngine;

public class SnappingPoints : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WZDraggable"))
        {
            if (!other.gameObject.TryGetComponent(out PaintingSnapping paint)) { return; }

            paint.SnapToAnyPoint();
        }
    }
}
