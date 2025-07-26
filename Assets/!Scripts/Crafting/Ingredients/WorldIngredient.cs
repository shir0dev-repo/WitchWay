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
    private Rigidbody[] Rigidbodies
    {
        get
        {
            if (_rb == null)
            {
                _rb = new Rigidbody[1];
                _rb[0] = GetComponent<Rigidbody>();
            }
            if (_rb[0] == null)
            {
                _rb = GetComponentsInChildren<Rigidbody>();
            }

            return _rb;
        }
    }
    private Rigidbody[] _rb = null;

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

        if (Rigidbodies.Length > 0)
        {
            foreach (Rigidbody rb in Rigidbodies)
            {
                rb.useGravity = false;
                if (!rb.isKinematic)
                    rb.linearVelocity = Vector3.zero;
            }
        }

        foreach (Collider c in Colliders) c.isTrigger = true;

        CursorManager.Instance.AttachToCursor(transform, transform);

        if (SoundManager.Instance != null && !_data.BaseIngredient.OnPickupAudioClip.IsNull)
            SoundManager.Instance.PlayOneShot(_data.BaseIngredient.OnPickupAudioClip, CursorManager.Instance.AttachedObject.transform.position);
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

        if (Rigidbodies.Length > 0)
        {
            foreach (Rigidbody rb in Rigidbodies)
            {
                rb.useGravity = true;
            }
        }

        foreach (Collider c in Colliders)
        {
            c.isTrigger = false;
        }

        if (SoundManager.Instance != null && !_data.BaseIngredient.OnPutDownAudioClip.IsNull)
            SoundManager.Instance.PlayOneShot(_data.BaseIngredient.OnPutDownAudioClip, CursorManager.Instance.AttachedObject.transform.position);

        CursorManager.Instance.ClearCursor();
    }
}