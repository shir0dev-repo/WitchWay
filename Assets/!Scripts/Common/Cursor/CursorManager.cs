using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    [SerializeField] private Camera _mainCam;
    [SerializeField] private bool _useDebug = true;

    private Transform _restPivot = null;

    public Transform AttachedObject => _attachedObject;
    private Transform _attachedObject = null;
    public bool HasObjectFollowingCursor => _isObjectAttached;
    private bool _isObjectAttached = false;
    public void ToggleVisibility(bool visible)
    {
        if (_useDebug) return;

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
        Vector3 oProjC = Vector3.Project(_attachedObject.position - _mainCam.transform.position, _mainCam.transform.forward);
        mousePos.z = oProjC.magnitude;// Mathf.Abs(_attachedObject.position.z - Camera.main.transform.position.z); //get how far the object is from the camera on z axis
        Vector3 worldMousePos = _mainCam.ScreenToWorldPoint(mousePos);

        _attachedObject.position = new Vector3(worldMousePos.x, worldMousePos.y, _attachedObject.position.z); //set new position (keeping the object's z axis)
    }

    public void AttachToCursor(Transform obj, Transform returnPivot)
    {
        if (AttachedObject != null) return;

        Debug.Log($"Attached {obj.name} to cursor!");
        _restPivot = returnPivot;
        _attachedObject = obj;
        ToggleVisibility(false);
        _isObjectAttached = true;

        if (_attachedObject.TryGetComponent(out ToolBase _))
        {
            Vector3 p = _attachedObject.position;
            _attachedObject.position = new(p.x, p.y, 0);
        }
    }

    public void AssignReturnPivot(Transform newPivot)
    {
        if (_attachedObject == null) return;

        _restPivot = newPivot;
    }

    public void ClearCursor(bool returnToRestPosition = true)
    {
        //Debug.Log($"Detached {_attachedObject.name} from cursor.");
        if (returnToRestPosition)
            _attachedObject.position = _restPivot.position;

        _restPivot = null;
        _attachedObject = null;

        _isObjectAttached = false;
        ToggleVisibility(true);
    }

    public static bool CastScreenRay(Vector2 mousePos, out RaycastHit hit, LayerMask layermask)
    {
        Ray r = Camera.main.ScreenPointToRay(mousePos);
        return Physics.Raycast(r, out hit, Mathf.Infinity, layermask);
    }
}
