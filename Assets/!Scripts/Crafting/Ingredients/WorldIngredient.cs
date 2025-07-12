using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class WorldIngredient : MonoBehaviour, IFollowCursor
{
    public void SetIngredient(IngredientSO data)
    {
        _data.BaseIngredient = data;
    }

    public IngredientSO BaseIngredient => _data.BaseIngredient; //added this so can ref what ingredient it is
    
    public ModifiedIngredient ModifiedState => _data;
    public void UpdateModifiers(ModifiedIngredient mod) => _data = mod;

    [SerializeField] private ModifiedIngredient _data;
    [HideInInspector] public bool _isDragging = false;

    [Header("Grabbing")]
    [SerializeField] private float baseDepth = 0f;
    [SerializeField] private float baseDepthDeviation;

    private static Camera MainCam
    {
        get
        {
            if (_cam == null)
            {
                _cam = Camera.main;
            }

            return _cam;
        }
    }

    private static Camera _cam = null;
    private Rigidbody Rigidbody
    { 
        get
        {
            if (_rb == null) _rb = GetComponent<Rigidbody>();
            return _rb;
        }
    }
    private Rigidbody _rb = null;

    private Collider[] Colliders
    {
        get
        {
            if (_colliders == null) _colliders = GetComponents<Collider>();
            return _colliders;
        }
    }
    private Collider[] _colliders = null;

    [HideInInspector] public float currentDepth;
    [HideInInspector] public Vector3 startPos = Vector3.zero;

    public void BeginDrag()
    {
        Ray ray = MainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.gameObject == gameObject)
            {
                startPos = hit.collider.transform.position;

                if (CursorManager.Instance == null) return;

                _isDragging = true;
                Rigidbody.useGravity = false;
                Rigidbody.linearVelocity = Vector3.zero;
                
                foreach (Collider c in Colliders) c.isTrigger = true;

                CursorManager.Instance.AttachToCursor(transform, transform);
            }
        }
    }

    public void UpdateDrag()
    {
        if (TryGetComponent(out CuttableIngredient cuttable))
            foreach (var segment in cuttable.Segments)
                segment.UpdateDrag();
    }

    public void EndDrag()
    {
        if (CursorManager.Instance == null) return;
        else if (CursorManager.Instance.AttachedObject != transform) return;

        _isDragging = false;

        if (TryGetComponent(out CuttableIngredient cuttable))
        {
            foreach (var segment in cuttable.Segments)
            {
                segment.Ungrab();
            }
        }

        if (Rigidbody != null) Rigidbody.useGravity = true;

        foreach (Collider c in Colliders)
        {
            c.isTrigger = false;
        }

        CursorManager.Instance.ClearCursor();
    }

    public void NoColliderBeginDrag(Transform supplyingCollider)
    {
        if (CursorManager.Instance == null) return;

        _isDragging = true;

        foreach (Rigidbody rbb in transform.GetComponentsInChildren<Rigidbody>())
        {
            rbb.useGravity = false;
            rbb.linearVelocity = Vector3.zero;
            rbb.angularVelocity = Vector3.zero;
        }

        foreach (Collider c in _colliders)
        {
            c.isTrigger = true;
        }

        CursorManager.Instance.AttachToCursor(transform, supplyingCollider);
    }
}