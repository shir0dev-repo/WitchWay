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

    [Header("Prep Board")]
    [SerializeField] private Transform _prepBoardArea;
    [SerializeField] private Transform _prepBoardPivot;

    [Header("Temperature Pot")]
    [SerializeField] private Transform _tempPotArea;
    [SerializeField] private Transform _tempPotPivot;

    [Header("Magic Circle")]
    [SerializeField] private Transform _magicCircleArea;
    [SerializeField] private Transform _arcaneCirclePivot;

    [Header("Cauldron")]
    [SerializeField] private Transform _cauldronArea;
    [SerializeField] private Transform _cauldronPivot;

    private int _currentTransformIndex = 0;

    public Transform CurrentStationArea { get; private set; }

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

    public void ToggleDrag(bool toggle)
    {
        canDrag = toggle;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        CurrentStationArea = _prepBoardArea;
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
            0 => _prepBoardPivot.position,  // Cutting Board
            1 => _tempPotPivot.position,  // Mortar and Pestle
            2 => _arcaneCirclePivot.position,   // Magic Circle
            3 => _cauldronPivot.position,           // Cauldron
            _ => Vector3.zero
        };

        CameraManager.Instance.MoveToPosition(targetPos);
        _currentTransformIndex = targetStation;
        GameEvents.Crafting.OnStationChanged?.Invoke(targetStation);
        CurrentStationArea = (targetStation) switch
        {
            0 => _prepBoardArea,
            1 => _tempPotArea,
            2 => _magicCircleArea,
            3 => _cauldronArea,
            _ => null
        };
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

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> uiHits = new List<RaycastResult>();
            GraphicRaycaster raycaster = FindFirstObjectByType<GraphicRaycaster>();
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

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //return Physics.Raycast(ray, out RaycastHit hit) && hit.collider == tableCollider;
        if (CursorManager.BlockInteraction) { return false; }

        if (CursorManager.CastScreenRay(Input.mousePosition, out RaycastHit hit))
        {
            return hit.collider == tableCollider;
        }
        return false;
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
                //transform.position = new Vector3(tableStartPos.x + (deltaX * 0.01f), tableTransform.position.y, tableTransform.position.z);
                CameraManager.Instance.SetPosition(new Vector3(tableStartPos.x + (deltaX * 0.01f), tableTransform.position.y, tableTransform.position.z));
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
            //transform.position = tableStartPos;
            CameraManager.Instance.SetPosition(tableStartPos);
        }
    }
    public int GetCurrentStation()
    {
        return _currentTransformIndex;
    }
}
