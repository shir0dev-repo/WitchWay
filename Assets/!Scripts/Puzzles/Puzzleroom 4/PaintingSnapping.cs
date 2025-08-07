using UnityEngine;

public class PaintingSnapping : MonoBehaviour
{
    [SerializeField] SnappableObject _snapping;
    
    [SerializeField] string hint; // for debugging purposes
    [SerializeField] Transform _correctPosition;

    public bool _isInCorrectPosition = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(hint);
        }
    }

    public SnappableObject GetSnappableObject() { return _snapping; }
    public void SnapToAnyPoint()
    {
        _snapping.SnapToPoint();
        gameObject.transform.rotation = _snapping.ClosestSnapPoint.rotation;

        CheckSnappedPosition();
    }
    void CheckSnappedPosition()
    {
        if (gameObject.transform == _correctPosition) { _isInCorrectPosition = true; }
        else { _isInCorrectPosition = false; }
    }
}
