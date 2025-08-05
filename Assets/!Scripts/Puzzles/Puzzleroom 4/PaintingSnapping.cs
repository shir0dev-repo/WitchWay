using UnityEngine;

public class PaintingSnapping : MonoBehaviour
{
    [SerializeField] SnappableObject _snappableObject;

    bool _canSnap = false;
    public bool _isInCorrectPosition = false;
    int _currentIndex;
    [SerializeField] Transform _correctPosition;

    private void Start()
    {
        //RandomizePosition();
        _currentIndex = 2;
        ChangeGameObjectRotation(_currentIndex);
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
        IterateThroughSnapPoints();
        Debug.Log("snapping to next cardinal position: " + _snappableObject.SnapPoints[_currentIndex].name);

        if (transform.localRotation == _correctPosition.localRotation)
        {
            Debug.Log("painting is in the right position!");
            _isInCorrectPosition = true;

            gameObject.GetComponentInParent<PuzzleRoomPainting>()?.IsSolved();
            return;
        }

        _isInCorrectPosition = false;
    }
    void RandomizePosition()
    {
        _currentIndex = Random.Range(0, _snappableObject.SnapPoints.Length - 1);
        ChangeGameObjectRotation(_currentIndex);
    }
    void IterateThroughSnapPoints()
    {
        _currentIndex++;

        if (_currentIndex > _snappableObject.SnapPoints.Length - 1)
        {
            _currentIndex = 0;
        }

        ChangeGameObjectRotation (_currentIndex);
    }
    void ChangeGameObjectRotation(int nextTransform)
    {
        Transform newTransform = _snappableObject.SnapPoints[nextTransform];

        gameObject.transform.rotation = newTransform.localRotation;
        // the gameobject as a whole will rotate according to the local rotation
        // that was set in the snap point's rotation (eg south = y180 (euler angles))
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
