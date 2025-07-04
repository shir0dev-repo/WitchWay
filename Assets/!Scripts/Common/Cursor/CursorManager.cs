using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    [SerializeField] private Camera _mainCam;
    [SerializeField] private bool _useDebug = true;

    private Transform _restPivot = null;

    private Vector3 _grabOffset = Vector3.zero;

    public Transform AttachedObject => _attachedObject;
    private IFollowCursor _currentFollowCursor = null;
    private Transform _attachedObject = null;
    
    private float _targetZPosition = 0.0f;
    private bool _attachedObjectInDestroyArea = false;

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

    private void Update()
    {
        if (!_isObjectAttached && Input.GetMouseButtonDown(0))
        {
            int layer = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
            if (CastScreenRay(Input.mousePosition, out RaycastHit hit, layer) && hit.transform.TryGetComponent(out _currentFollowCursor))
            {
                _currentFollowCursor.BeginDrag();
            }
        }

        if (_isObjectAttached && Input.GetMouseButtonUp(0))
        {
            _currentFollowCursor?.EndDrag();
        }

        if (_currentFollowCursor != null)
        {
            _currentFollowCursor.UpdateDrag();
        }
    }

    private void FixedUpdate()
    {
        if (_isObjectAttached)
        {
            TryGetZTarget();
            SnapCurrentObjectToCursor();
        }
    }

    private void TryGetZTarget()
    {
        _targetZPosition = 0.0f;

        if (!_attachedObject.TryGetComponent(out WorldIngredient ing)) return;
        if (StationsInventory.Instance == null) return;

        CraftingRectArea[] craftingRects = StationsInventory.Instance.GetCraftingRects();

        if (craftingRects == null) return;

        for (int i = 0; i < craftingRects.Length; i++)
        {
            CraftingRectArea craftingRectArea = craftingRects[i];
            RectTransform rect = craftingRectArea.screenRect;

            Vector2 localMousePosition = rect.InverseTransformPoint(Input.mousePosition);
            if (rect.rect.Contains(localMousePosition))
            {
                _targetZPosition = craftingRectArea.depthValue;
                _attachedObjectInDestroyArea = i == StationsInventory.Instance.DestroySectionIndex;
                break;
            }
        }
    }

    public Vector3 GetMouseWorldPos()
    {
        if (_attachedObject == null) return Vector3.zero;

        Vector3 mousePos = Input.mousePosition;
        float depth = Vector3.Dot(_attachedObject.position - _mainCam.transform.position, _mainCam.transform.forward);

        mousePos.z = depth;
        Vector3 w = _mainCam.ScreenToWorldPoint(mousePos);
        w.z = _targetZPosition;

        return w;
    }

    public Vector3 GetMouseWorldPos(Transform relativeDepth)
    {
        Vector3 mousePos = Input.mousePosition;
        float depth = Vector3.Dot(relativeDepth.position - _mainCam.transform.position, _mainCam.transform.forward);

        mousePos.z = depth;
        Vector3 w = _mainCam.ScreenToWorldPoint(mousePos);
        w.z = _targetZPosition;

        return w;
    }

    private void SnapCurrentObjectToCursor()
    {
        //get the position of the mouse
        /*Vector3 mousePos = Input.mousePosition;

        Vector3 oProjC = Vector3.Project(_attachedObject.position - _mainCam.transform.position, _mainCam.transform.forward);

        // /get how far the object is from the camera on z axis
        // Mathf.Abs(_attachedObject.position.z - Camera.main.transform.position.z); /

        mousePos.z = oProjC.magnitude;
*/
        Vector3 worldMousePos = GetMouseWorldPos();

        //set new position (keeping the object's z axis)
        _attachedObject.position = worldMousePos;
        Debug.DrawLine(_mainCam.transform.position, worldMousePos, Color.red);
    }

    public void AttachToCursor(Transform obj, Transform returnPivot, Vector3 grabOffset)
    {
        AssignCursorOffset(grabOffset);
        AttachToCursor(obj, returnPivot);
    }

    public void AttachToCursor(Transform obj, Transform returnPivot, bool useOffset = true)
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
        else if (_attachedObject.TryGetComponent(out WorldIngredient _) && _attachedObject.TryGetComponent(out Collider col))
        {
            col.isTrigger = true;
        }
    }

    public void AssignReturnPivot(Transform newPivot)
    {
        if (_attachedObject == null) return;

        _restPivot = newPivot;
    }

    public void AssignCursorOffset(Vector3 offset) => _grabOffset = offset;

    public void ClearCursor(bool returnToRestPosition = true)
    {
        if (_attachedObject == null) return;

        _grabOffset = Vector3.zero;
        _currentFollowCursor = null;

        if (returnToRestPosition && _restPivot != null)
            _attachedObject.position = _restPivot.position;
        
        if (_attachedObject.TryGetComponent(out WorldIngredient ing))
        {
            if (_attachedObjectInDestroyArea)
            {
                GameEvents.Crafting.OnItemPlacedInTrash?.Invoke(ing);
                Destroy(_attachedObject.gameObject);
            }
        }

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
