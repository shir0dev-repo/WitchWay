using UnityEngine;

public class PaintingSnapping : MonoBehaviour
{
    [SerializeField] SnappableObject _snapping;
    
    [SerializeField] string hint; // for debugging purposes
    [SerializeField] Transform _correctPosition;

    PuzzleRoomPainting room;
    WZPlayerInteract playerInteract;

    public bool _isInCorrectPosition = false;
    bool _isCurrentlyGrabbing = false;

    public bool IsCurrentlyGrabbing => _isCurrentlyGrabbing;

    private void Start()
    {
        room = GetComponentInParent<PuzzleRoomPainting>();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerInteract = player.GetComponent<WZPlayerInteract>();
    }
    private void Update()
    {
        if (_isCurrentlyGrabbing && Input.GetMouseButtonUp(0))
        {
            SnapToAnyPoint();
            _isCurrentlyGrabbing = false;
        }

        if (!CheckIfThisIsCurrentlyGrabbedObject()) { return; }
        
        if (Input.GetMouseButtonDown(0)) { Debug.Log(hint); }
        if (Input.GetMouseButton(0)) { _isCurrentlyGrabbing = true; }
    }
    public void SnapToAnyPoint()
    {
        _snapping.SnapToPoint();
        gameObject.transform.rotation = _snapping.ClosestSnapPoint.rotation;

        CheckSnappedPosition();
    }
    void CheckSnappedPosition()
    {
        if (gameObject.transform.position == _correctPosition.position) 
        { 
            _isInCorrectPosition = true;
            room.AddToCorrectPaintings();
        }
        else { _isInCorrectPosition = false; }
    }
    bool CheckIfThisIsCurrentlyGrabbedObject()
    {
        GameObject o = playerInteract.GetCurrentlyDraggedObject();

        if (o != null && o == gameObject)
        {
            return true;
        }

        _isCurrentlyGrabbing = false;
        return false;
    }
    public SnappableObject GetSnappableObject() { return _snapping; } 
}
