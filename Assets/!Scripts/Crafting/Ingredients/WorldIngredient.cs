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
        if (CursorManager.Instance == null) return;

        _isDragging = true;

        if (Rigidbody != null)
        {
            Rigidbody.useGravity = false;
            Rigidbody.linearVelocity = Vector3.zero;
        }

        foreach (Collider c in Colliders) c.isTrigger = true;

        CursorManager.Instance.AttachToCursor(transform, transform);
    }

    public void EndDrag()
    {
        if (CursorManager.Instance == null) return;
        else if (CursorManager.Instance.AttachedObject != transform) return;

        _isDragging = false;
        Rigidbody.useGravity = true;

        foreach (Collider c in Colliders)
        {
            c.isTrigger = false;
        }

        CursorManager.Instance.ClearCursor();
    }
}