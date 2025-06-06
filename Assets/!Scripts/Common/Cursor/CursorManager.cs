using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    [SerializeField] private Camera _mainCam;

    private Transform _restPivot = null;
    private Transform _attachedObject = null;

    public bool HasObjectFollowingCursor => _isObjectAttached;
    private bool _isObjectAttached = false;
    public void ToggleVisibility(bool visible)
    {
        Cursor.visible = visible;
    }

    protected override void Awake()
    {
        base.Awake();
        if (_mainCam == null)
            _mainCam = Camera.main;
    }

    private void FixedUpdate()
    {
        if (_isObjectAttached)
        {
            SnapCurrentObjectToCursor();
        }
    }

    private void SnapCurrentObjectToCursor()
    {
        Vector3 mousePos = Input.mousePosition; //get the position of the mouse

        mousePos.z = Mathf.Abs(Camera.main.transform.position.z - _attachedObject.position.z); //get how far the object is from the camera on z axis
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

        _attachedObject.position = new Vector3(worldMousePos.x, worldMousePos.y, _attachedObject.position.z); //set new position (keeping the object's z axis)
    }

    public void AttachToCursor(Transform obj, Transform returnPivot)
    {
        Debug.Log($"Attached {obj.name} to cursor!");
        _restPivot = returnPivot;
        _attachedObject = obj;
        ToggleVisibility(false);
        _isObjectAttached = true;
    }

    public void ClearCursor()
    {
        //Debug.Log($"Detached {_attachedObject.name} from cursor.");
        _attachedObject.position = _restPivot.position;
        _restPivot = null;
        _attachedObject = null;

        _isObjectAttached = false;
        ToggleVisibility(true);
    }
}
