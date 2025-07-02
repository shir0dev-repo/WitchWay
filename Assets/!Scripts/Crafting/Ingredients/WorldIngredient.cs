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

    private static Camera _cam = null;

    private Rigidbody rb;
    private Collider[] _colliders;

    [HideInInspector] public float currentDepth;
    [HideInInspector] public Vector3 startPos = Vector3.zero;

    private void Start()
    {
        if (_cam == null)
            _cam = Camera.main;
        
        rb = GetComponent<Rigidbody>();
        _colliders = GetComponents<Collider>();
    }

    public void BeginDrag()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.gameObject == gameObject)
            {
                startPos = hit.collider.transform.position;

                if (CursorManager.Instance == null) return;

                _isDragging = true;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                
                foreach (Collider c in _colliders) c.isTrigger = true;

                CursorManager.Instance.AttachToCursor(transform, transform);
            }
        }
    }

    public void EndDrag()
    {
        if (CursorManager.Instance == null) return;
        else if (CursorManager.Instance.AttachedObject != transform) return;

        _isDragging = false;
        rb.useGravity = true;

        foreach (Collider c in _colliders)
        {
            c.isTrigger = false;
        }

        CursorManager.Instance.ClearCursor();
    }
}