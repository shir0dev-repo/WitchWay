using UnityEngine;

public class PaintingSnapping : MonoBehaviour
{
    [SerializeField] SnappableObject _snapping;
    
    [SerializeField] string hint; // for debugging purposes
    [SerializeField] Transform _correctPosition;

    public bool _isInCorrectPosition = false;
    bool _isCurrentlyGrabbing = false;

    public bool IsCurrentlyGrabbing => _isCurrentlyGrabbing;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) { Debug.Log(hint); }
        if (Input.GetMouseButton(0)) { _isCurrentlyGrabbing = true; }
        if (Input.GetMouseButtonUp(0)) { _isCurrentlyGrabbing = false;}
    }
    public void SnapToAnyPoint()
    {
        _snapping.SnapToPoint();
        gameObject.transform.rotation = _snapping.ClosestSnapPoint.rotation;

        CheckSnappedPosition();
    }
    void CheckSnappedPosition()
    {
        if (gameObject.transform.position == _correctPosition.position) { _isInCorrectPosition = true; }
        else { _isInCorrectPosition = false; }
    }
    public SnappableObject GetSnappableObject() { return _snapping; }
}
