using UnityEngine;

public class SnappingPoints : MonoBehaviour
{
    PaintingSnapping _currentPainting;

    private void Update()
    {
        if (_currentPainting != null && !_currentPainting.IsCurrentlyGrabbing)
        {
            _currentPainting.SnapToAnyPoint();
            _currentPainting = null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WZDraggable") && other.TryGetComponent(out PaintingSnapping paint))
        {
            _currentPainting = paint;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PaintingSnapping paint) && paint == _currentPainting)
        {
            _currentPainting = null;
        }
    }
}
