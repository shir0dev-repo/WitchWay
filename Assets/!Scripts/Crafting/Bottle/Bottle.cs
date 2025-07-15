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
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        CursorManager.Instance.AttachToCursor(transform, transform);
    }

    public void EndDrag()
    {
        if (CursorManager.Instance == null || CursorManager.Instance.AttachedObject != transform) return;

        CursorManager.Instance.ClearCursor(false);

        if (BottlingStation.Instance != null && BottlingStation.Instance.CurrentBottle == this)
        {
            GameEvents.Crafting.OnBottlePlacedInBottler?.Invoke(this);
            transform.position = BottlingStation.Instance.BottlePivot.position;
        }
        else
        {
            _collider.isTrigger = false;
            _rb.useGravity = true;
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        
    }
}
