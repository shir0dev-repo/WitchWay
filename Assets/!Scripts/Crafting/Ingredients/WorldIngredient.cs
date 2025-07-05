using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class WorldIngredient : MonoBehaviour
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

    private Vector3 _mousePosWS = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private float _moveSpeed = 4.5f;

    [Header("Grabbing")]
    [SerializeField] private float baseDepth = 0f;
    [SerializeField] private float baseDepthDeviation;

    private static Camera _cam = null;

    private Rigidbody rb;
    private float currentDepth;
    private bool isStationValid = true;
    private bool inDestroyArea = false;

    [HideInInspector] public Vector3 startPos = Vector3.zero;

    //private StationManager stationManager;

    private void OnEnable()
    {
        GameEvents.Crafting.OnItemPlacedInStation += CheckValid;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnItemPlacedInStation -= CheckValid;
    }

    private void Start()
    {
        if (_cam == null)
            _cam = Camera.main;

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleInput();

        if (_isDragging)
        {
            HandleScroll();
            UpdateDragging();
        }
    }

    private void HandleInput()
    {
        if (!_isDragging && Input.GetMouseButtonDown(0))
        {
            CastRay();
        }
        else if (_isDragging && Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    private void HandleScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentDepth += scroll * 5f;

            float maxDepth = baseDepth + baseDepthDeviation;
            currentDepth = Mathf.Clamp(currentDepth, baseDepth, maxDepth);
        }
    }

    private void CastRay()
    {
        if (CursorManager.BlockInteraction)
        {
            return;
        }

        /*Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject == gameObject)
            {
                startPos = hit.collider.transform.position;
                BeginDrag();
            }
        }*/

        if (CursorManager.CastScreenRay(Input.mousePosition, out RaycastHit hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                startPos = hit.collider.transform.position;
                BeginDrag();
            }
        }
    }

    private void BeginDrag()
    {
        _isDragging = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        if (CursorManager.Instance != null)
            CursorManager.Instance.AttachToCursor(transform, transform);
    }

    private void UpdateDragging()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 oProjC = Vector3.Project(transform.position - _cam.transform.position, _cam.transform.forward);
        mousePos.z = oProjC.magnitude;

        Vector3 worldPos = _cam.ScreenToWorldPoint(mousePos);

        if (StationsInventory.Instance != null)
        {
            currentDepth = baseDepth;
            CraftingRectArea[] craftingRects = StationsInventory.Instance.GetCraftingRects();

            if (craftingRects != null)
            {
                for (int i = 0; i < craftingRects.Length; i++)
                {
                    CraftingRectArea craftingRectArea = craftingRects[i];
                    RectTransform rect = craftingRectArea.screenRect;

                    Vector2 localMousePosition = rect.InverseTransformPoint(mousePos);
                    if (rect.rect.Contains(localMousePosition))
                    {
                        currentDepth = craftingRectArea.depthValue;
                        inDestroyArea = i == StationsInventory.Instance.DestroySectionIndex;

                        break;
                    }
                }
            }
        }

        _mousePosWS = new Vector3(worldPos.x, worldPos.y, currentDepth);
        transform.position = Vector3.SmoothDamp(transform.position, _mousePosWS, ref _velocity, _moveSpeed * Time.deltaTime);
    }

    public void EndDrag()
    {
        _isDragging = false;
        rb.useGravity = true;

        if (inDestroyArea)
        {
            FindFirstObjectByType<StationsInventory>().PermanentRemove(this);
            Destroy(gameObject);
        }
        else if (CursorManager.Instance != null)
        {
            if (CursorManager.Instance.AttachedObject == transform)
                CursorManager.Instance.ClearCursor();
        }
    }

    private void CheckValid(WorldIngredient wIngredient, StationType station, Transform stationAnchor)
    {
        if (wIngredient != this) return;

        if (BaseIngredient == null)
        {
            StartCoroutine(DeferredCheck(station));
            return;
        }

        isStationValid = BaseIngredient.CanBeUsedAtStation(station);
        if (isStationValid)
        {
            transform.position = stationAnchor.position;
        }
    }

    private IEnumerator DeferredCheck(StationType station)
    {
        yield return null;
        if (BaseIngredient != null)
            isStationValid = BaseIngredient.CanBeUsedAtStation(station);
    }
}