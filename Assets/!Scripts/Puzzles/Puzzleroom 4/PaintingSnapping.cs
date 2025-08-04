using UnityEngine;

public class PaintingSnapping : MonoBehaviour
{
    [SerializeField] SnappableObject _snappableObject;

    bool _canSnap = false;
    int _currentIndex;
    [SerializeField] Transform _correctPosition;
    private void Start()
    {
        //RandomizePosition();
        _currentIndex = 0;
        ChangeGameObjectTransform(_currentIndex);
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
        RotatePaintingToNextSnapPoint();
        Debug.Log("snapping to next cardinal position: " + _snappableObject.SnapPoints[_currentIndex].name);

        if (transform.localRotation == _correctPosition.localRotation)
        {
            Debug.Log("painting is in the right position!");
        }
    }
    void RandomizePosition()
    {
        _currentIndex = Random.Range(0, _snappableObject.SnapPoints.Length - 1);
        ChangeGameObjectTransform(_currentIndex);
    }
    void RotatePaintingToNextSnapPoint()
    {
        _currentIndex++;

        if (_currentIndex > _snappableObject.SnapPoints.Length - 1)
        {
            _currentIndex = 0;
        }

        ChangeGameObjectTransform (_currentIndex);
    }
    void ChangeGameObjectTransform(int nextTransform)
    {
        Transform newTransform = _snappableObject.SnapPoints[nextTransform];

        gameObject.transform.rotation = newTransform.localRotation;
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
