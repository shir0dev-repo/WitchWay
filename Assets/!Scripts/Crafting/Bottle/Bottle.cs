using System;
using UnityEngine;

public class Bottle : MonoBehaviour, IFollowCursor
{
    [SerializeField] private BottleVisuals _bottleVisuals;

    private Collider _collider;
    private Rigidbody _rb;

    public bool CanBeBottled { get; private set; }

    private void OnEnable()
    {
        CanBeBottled = true;
        GameEvents.Crafting.OnBottleFilled += CapBottle;
    }

    private void CapBottle()
    {
        CanBeBottled = false;
    }

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

        AdjustPhysicsBehaviour(true);
        CursorManager.Instance.AttachToCursor(transform, transform);
    }

    public void EndDrag()
    {
        if (CursorManager.Instance == null || CursorManager.Instance.AttachedObject != transform) return;

        CursorManager.Instance.ClearCursor(false);

        if (BottlingStation.Instance == null || BottlingStation.Instance.CurrentBottle != this)
        {
            AdjustPhysicsBehaviour(false);
        }
    }

    private void AdjustPhysicsBehaviour(bool isCurrentlyHeld)
    {
        _collider.isTrigger = isCurrentlyHeld;
        _rb.useGravity = !isCurrentlyHeld;
        _rb.constraints = isCurrentlyHeld ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.FreezeRotation;
    }
}
