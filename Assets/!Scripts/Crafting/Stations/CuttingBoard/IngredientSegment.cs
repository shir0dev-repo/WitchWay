
using System.Collections.Generic;
using UnityEngine;

public class IngredientSegment : MonoBehaviour, IFollowCursor
{
    public bool HasBeenDetached { get; private set; } = false;
    public Vector3 Center { get; private set; } = Vector3.zero;
    public float GrabVelocity { get; set; } = 0.0f;
    public float MaxGrabVelocity { get; set; } = 0.0f;

    private Rigidbody _rb;
    private RigidbodyConstraints _rbConstraints;
    private WorldIngredient _parentIngredient;

    private void Awake()
    {
        _parentIngredient = GetComponentInParent<WorldIngredient>();
        _rb = GetComponent<Rigidbody>();
        _rbConstraints = _rb.constraints;
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        if (TryGetComponent(out MeshRenderer mr))
            Center = mr.bounds.center;
    }

    public void Detach()
    {
        HasBeenDetached = true;
        _rb.constraints = _rbConstraints;
        _rb.isKinematic = false;
    }

    public List<IngredientSegment> GrabSimilar(Transform parent)
    {
        List<IngredientSegment> similar = new();
        foreach (Transform child in parent.transform)
        {
            if (child.TryGetComponent(out IngredientSegment segment))
                similar.Add(segment);
        }

        if (parent == transform) similar.Add(this);

        return similar;
    }

    private void Grab()
    {
        _rb.useGravity = false;
        
        _rb.excludeLayers = ~(1 << LayerMask.NameToLayer("Cursor Collection"));
    }

    private void Ungrab()
    {
        //transform.SetParent(_parentIngredient.transform);
        _rb.useGravity = true;
        _rb.excludeLayers = 0;
        _rb.includeLayers = 0;
    }

    public void BeginDrag()
    {
        if (CursorManager.Instance == null) return;

        CursorManager.Instance.AttachToCursor(transform, transform);

        var siblings = GrabSimilar(_parentIngredient.transform);
        
        foreach (IngredientSegment segment in siblings)
        {
            segment.Grab();
        }
    }
    
    public void UpdateDrag()
    {
        if (CursorManager.Instance == null) return;
        else if (CursorManager.Instance.AttachedObject != transform) return;

        var siblings = GrabSimilar(_parentIngredient.transform);
        foreach (IngredientSegment child in siblings)
        {
            if (child.transform == transform) continue;

            if (child.TryGetComponent(out Rigidbody rgbd))
            {
                Vector3 force = (transform.position - rgbd.position).normalized * GrabVelocity;
                rgbd.AddForce(force);
                rgbd.linearVelocity = Vector3.ClampMagnitude(rgbd.linearVelocity, MaxGrabVelocity);
            }
        }
    }

    public void EndDrag()
    {
        if (CursorManager.Instance == null) return;

        CursorManager.Instance.ClearCursor(false);
        var siblings = GrabSimilar(_parentIngredient.transform);

        foreach (IngredientSegment segment in siblings)
        {
            segment.Ungrab();
        }
    }
}
