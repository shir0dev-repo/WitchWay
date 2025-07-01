using UnityEngine;

public class Bottle : MonoBehaviour, IFollowCursor
{
    [SerializeField] private BottleVisuals _bottleVisuals;

    private Collider _collider;
    private Rigidbody _rb;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();

        if (_bottleVisuals == null)
            _bottleVisuals = GetComponent<BottleVisuals>();
    }

    public void BeginDrag()
    {
        if (CursorManager.Instance == null) return;

        _collider.isTrigger = true;
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        CursorManager.Instance.AttachToCursor(transform, transform, _bottleVisuals.GetPivotForHolder());
    }

    public void EndDrag()
    {
        if (CursorManager.Instance == null) return;
        if (CursorManager.Instance.AttachedObject == transform)
        {
            _collider.isTrigger = false;
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            CursorManager.Instance.ClearCursor(false);
        }
    }
}
