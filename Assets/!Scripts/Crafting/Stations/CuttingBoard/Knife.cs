using UnityEngine;

public class Knife : ToolBase
{
    Rigidbody rb;

    [SerializeField] Transform startPos;
    [SerializeField] Vector3 cutRotationEulers;
    [SerializeField] Vector3 restRotationEulers;

    bool isSelected = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!IsSelected && Input.GetMouseButtonDown(0) && CastRay())
        {
            
        }
        else if (IsSelected && Input.GetMouseButtonDown(1))
        {
            
        }
    }
    void RotateToRestPosition()
    {
        gameObject.transform.rotation = Quaternion.Euler(restRotationEulers);
    }
    void RotateToCuttingPosition()
    { // rotates the knife by 90 degrees on the z axis
        gameObject.transform.rotation = Quaternion.Euler(cutRotationEulers);
    }
    bool CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.rigidbody == rb) { return true; }
        }

        return false;
    }

    protected override void OnToolSelected()
    {
        isSelected = true;
        RotateToCuttingPosition();
        CursorManager.Instance.AttachToCursor(transform, startPos.position);
        GameEvents.Crafting.OnToolSelected?.Invoke(ToolType.Knife);
    }

    protected override void OnToolDeselected()
    {
        isSelected = false;
        RotateToRestPosition();
        CursorManager.Instance.ClearCursor();
        GameEvents.Crafting.OnToolDeselected?.Invoke(ToolType.Knife);
    }
}
