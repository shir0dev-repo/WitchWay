using UnityEngine;

public class PaintingSnapping : MonoBehaviour
{
    [SerializeField] public SnappableObject _snappable;
    int _snapPointNum = 0;

    void RotateToNextSnapPoint()
    {
        if (_snapPointNum < _snappable.SnapPoints.Length - 1) // so it goes like 0,1,2,3
        {
            _snapPointNum++;
            gameObject.transform.rotation = _snappable.SnapPoints[_snapPointNum].rotation;
            
        }
        else
        {
            _snapPointNum = 0;
            gameObject.transform.rotation = _snappable.SnapPoints[_snapPointNum].rotation;
        }
    }
}
