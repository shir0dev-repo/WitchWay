using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class StationManager : MonoBehaviour
{
    public const int _STATION_COUNT = 4;
    public static StationManager Instance { get; private set; }

    [Serializable]
    public class StationChangedEvent : UnityEvent<int> { }
    public StationChangedEvent OnStationChanged = new StationChangedEvent();

    [Header("Controls")]
    [SerializeField] private InputAction _changeStationAction;
    
    [Header("Cutting Board")]
    [SerializeField] private CuttingBoard _cuttingBoard;
    [SerializeField] private Transform _cuttingBoardTransform;

    [Header("Mortar & Pestle")]
    [SerializeField] private Pestle _pestle;
    [SerializeField] private Transform _mortarPestleTransform;

    [Header("Magic Circle")]
    [SerializeField] private SymbolPainter _magicCircle;
    [SerializeField] private Transform _magicCircleTransform;

    [Header("Cauldron")]
    [SerializeField] private Cauldron _cauldron;
    [SerializeField] private Transform _cauldronArea;

    private int _currentTransformIndex = 0;

    [HideInInspector]
    public bool recipeBookOpen = false;

    [Header("Drag Detection")]
    [SerializeField] private bool _useCameraSmoothing = false;
    [SerializeField] private float stationDragThreshold;
    [SerializeField] private float stationDragThresholdVisual;
    [SerializeField] private Transform tableTransform;
    [SerializeField] private Collider tableCollider;

    private Vector2 dragStartPos, dragEndPos, tableStartPos;
    private bool isDragging = false, clickedOnTable = false, canDrag = true;

    public void ToggleDrag(int stationID)
    {
        canDrag = stationID != 2;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        _changeStationAction.started += MoveToStation;
        _changeStationAction.Enable();
    }

    private void OnDisable()
    {
        _changeStationAction.started -= MoveToStation;
        _changeStationAction.Disable();
    }

    // "changing" station at the start for UI purposes
    private void Start()
    {
        OnStationChanged.Invoke(0);
    }

    public void GoPreviousStation()
    {
        _currentTransformIndex--;

        if (_currentTransformIndex < 0)
            _currentTransformIndex = _STATION_COUNT - 1;

        SwapStation(_currentTransformIndex);
    }

    public void GoNextStation()
    {
        _currentTransformIndex = (_currentTransformIndex + 1) % _STATION_COUNT;
        SwapStation(_currentTransformIndex);
    }


    public void SwapStation(int targetStation)
    {
        Vector3 targetPos = (targetStation) switch
        {
            0 => _cuttingBoardTransform.position,  // Cutting Board
            1 => _mortarPestleTransform.position,  // Mortar and Pestle
            2 => _magicCircleTransform.position,   // Magic Circle
            3 => _cauldronArea.position,           // Cauldron
            _ => Vector3.zero
        };

        CameraManager.Instance.MoveToPosition(targetPos);
        _currentTransformIndex = targetStation;
        OnStationChanged.Invoke(targetStation);
    }

    private void MoveToStation(InputAction.CallbackContext context)
    {
        // if the recipe book is open, don't change stations
        if (recipeBookOpen)
            return;

        float input = context.ReadValue<float>();

        if (input < 0)
            GoPreviousStation();
        else if (input > 0)
            GoNextStation();
    }
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;

    // Dragging logic
    private bool ClickedOnTable()
    {
        if (!canDrag)
            return false;

        // Check if pointer is over UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Perform UI raycast to check the exact UI element
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> uiHits = new List<RaycastResult>();
            GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>(); // Or cache it if performance matters
            raycaster.Raycast(pointerData, uiHits);

            foreach (RaycastResult result in uiHits)
            {
                // Only block if the UI element is NOT tagged to be ignored
                if (!result.gameObject.CompareTag("IgnoreUIDrag"))
                {
                    Debug.Log($"Blocked by UI element: {result.gameObject.name}");
                    return false;
                }
            }
        }

    // Proceed with table raycast
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    return Physics.Raycast(ray, out RaycastHit hit) && hit.collider == tableCollider;
    }


    //this block of code is for not changing stations until the mouse is let go
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // check that the table was clicked on
            if (ClickedOnTable())
            {
                isDragging = true;
                clickedOnTable = true;
                dragStartPos = Input.mousePosition;
                
                bool camSmooth = _useCameraSmoothing && CameraManager.Instance.IsMoving;
                tableStartPos = camSmooth ? CameraManager.Instance.GetTargetPosition() : tableTransform.position;
            }
        }

        if (Input.GetMouseButton(0) && isDragging && clickedOnTable)
        {
            Vector2 currentMousePos = Input.mousePosition;
            float deltaX = currentMousePos.x - dragStartPos.x;

            // a minimum offset to prevent small movements
            if (Mathf.Abs(deltaX) > stationDragThresholdVisual)
            {
                tableTransform.position = new Vector3(tableStartPos.x + (deltaX * 0.01f), tableTransform.position.y, tableTransform.position.z);
            }

        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;

            if (clickedOnTable)
            {
                dragEndPos = Input.mousePosition;
                DetectDragDirection();
            }

            clickedOnTable = false;
        }
    }

    private void DetectDragDirection()
    {
        float dragDistance = dragEndPos.x - dragStartPos.x;

        if (Mathf.Abs(dragDistance) > stationDragThreshold)
        {
            if (dragDistance > 0)
                GoPreviousStation();
            else
                GoNextStation();
        }
        else if (_useCameraSmoothing)
        {
            CameraManager.Instance.MoveToPosition(tableStartPos);
        }
        else
        {
            tableTransform.position = tableStartPos;
        }
    }
}
