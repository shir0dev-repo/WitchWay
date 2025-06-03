using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldIngredient : MonoBehaviour
{
    public IngredientSO ingredient; //added this so can ref what ingrediant it is

    [HideInInspector] public bool _isDragging = false;

    private Vector3 _mousePosWS = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private float _moveSpeed = 4.5f;

    [Header("Grabbing")]
    [SerializeField] private float baseDepth = 0f;
    [SerializeField] private float baseDepthDeviation;

    [Header("Depth Sections")]
    [SerializeField] private DepthSectionsConfig depthSectionsConfig;
    [SerializeField] private int destorySectionindex = 0;

    private static Camera _cam = null;

    private Rigidbody rb;
    private float currentDepth;
    private bool isStationValid = true;
    private bool inDestoryArea = false;
    [HideInInspector] public Vector3 startPos = Vector3.zero;

    private StationManager stationManger;

    private void Start()
    {
        if (_cam == null)
            _cam = Camera.main;
        //GetComponent<MeshRenderer>().material.color = Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);

        rb = GetComponent<Rigidbody>();

        stationManger = StationManager.Instance;
        if (stationManger != null) stationManger.OnStationChanged.AddListener(CheckValid);
    }

    private void Update()
    {
        //if (!_isDragging) return;

        //transform.position = Vector3.SmoothDamp(transform.position, _mousePosWS, ref _velocity, _moveSpeed * Time.deltaTime);

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
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
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
        //currentDepth = baseDepth;
    }

    private void UpdateDragging()
    {
        Vector3 mousePos = Input.mousePosition;

        currentDepth = baseDepth;

        if (depthSectionsConfig != null)
        {
            for (int i = 0; i < depthSectionsConfig.screenSections.Length; i++)
            {
                DepthChangeSections section = depthSectionsConfig.screenSections[i];
                if (section.screenRect.Contains(new Vector2(mousePos.x, mousePos.y)))
                {
                    currentDepth = section.depthValue;

                    if (i == destorySectionindex) inDestoryArea = true;
                    else inDestoryArea = false;

                    break;
                }
            }
        }

        Vector3 oProjC = Vector3.Project(transform.position - _cam.transform.position, _cam.transform.forward);
        mousePos.z = oProjC.magnitude;
        Vector3 worldPos = _cam.ScreenToWorldPoint(mousePos);
        _mousePosWS = new Vector3(worldPos.x, worldPos.y, currentDepth);

        transform.position = Vector3.SmoothDamp(transform.position, _mousePosWS, ref _velocity, _moveSpeed * Time.deltaTime);
    }

    private void EndDrag()
    {
        _isDragging = false;
        rb.useGravity = true;

        if (inDestoryArea)
        {
            FindFirstObjectByType<StationsInventory>().PermanentRemove(this);
            Destroy(gameObject);
        }
        else if (!isStationValid)
        {
            transform.position = startPos;
        }
    }

    private void CheckValid(int stationId)
    {
        if (ingredient == null)
        {
            StartCoroutine(DeferredCheck(stationId));
            return;
        }
        isStationValid = ingredient.CanBeUsedAtStation(stationId);
    }

    private IEnumerator DeferredCheck(int stationId)
    {
        yield return null;
        if (ingredient != null)
            isStationValid = ingredient.CanBeUsedAtStation(stationId);
    }
}