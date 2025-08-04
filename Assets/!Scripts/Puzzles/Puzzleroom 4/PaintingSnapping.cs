using UnityEngine;

public class PaintingSnapping : MonoBehaviour
{
    [SerializeField] SnappableObject _snappableObject;

    bool _canSnap = false;
    [SerializeField] Transform _correctPosition;
    private void Start()
    {
        RandomizePosition();
    }

    private void Update()
    {
        if (!_canSnap) { return; }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RotatePaintingToNextDirection();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canSnap = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canSnap = false;
        }
    }
    void RotatePaintingToNextDirection()
    {
        Debug.Log("snapping to next cardinal position, not acutally tho");

        if (transform == _correctPosition)
        {
            PuzzleRoomPainting o = gameObject.transform.GetComponentInParent<PuzzleRoomPainting>();
            if (o != null) 
            {
                o.IsSolved();
            }
        }
    }
    void RandomizePosition()
    {
        Transform newTransform = _snappableObject.SnapPoints[Random.Range(0, 3)];

        if (newTransform == _correctPosition)
        {
            newTransform = _snappableObject.SnapPoints[0];
        }

        gameObject.transform.position = newTransform.position;
        gameObject.transform.rotation = newTransform.rotation;
    }
    public Transform GetCorrectPosition()
    {
        return _correctPosition;
    }
    public SnappableObject GetSnappableObject()
    {
        return _snappableObject;
    }
}
