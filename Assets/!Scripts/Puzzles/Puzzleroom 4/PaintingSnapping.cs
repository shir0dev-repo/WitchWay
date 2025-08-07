using UnityEngine;

public class PaintingSnapping : MonoBehaviour
{
    [SerializeField] SnappableObject _snapping;
    
    [SerializeField] string hint; // for debugging purposes
    [SerializeField] Transform _correctPosition;

    public bool _isInCorrectPosition = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(hint);
        }
    }

    public SnappableObject GetSnappableObject() { return _snapping; }
}
