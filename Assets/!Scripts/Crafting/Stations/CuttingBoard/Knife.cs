using UnityEngine;

public class Knife : MonoBehaviour
{
    InvisibleCursor cursorVisibility;
    FollowMouse FollowMouse;
    Rigidbody rb;

    [SerializeField] Transform startPos;
    [SerializeField] Vector3 cutRotationEulers;
    [SerializeField] Vector3 restRotationEulers;
    Quaternion startRot;

    bool isCursorVisible = true;
    void Start()
    {
        cursorVisibility = GetComponent<InvisibleCursor>();
        FollowMouse = GetComponent<FollowMouse>();
        rb = GetComponent<Rigidbody>();

        startRot = gameObject.transform.rotation;
    }

    void Update()
    {
        if (isCursorVisible)
        {
            if (Input.GetMouseButtonDown(0) && CastRay())
            {
                cursorVisibility.TurnCursorInsivible();
                isCursorVisible = false;
                RotateToCuttingPosition();
                CuttingBoard.Instance.ChangeCuttingAbility();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                CuttingBoard.Instance.ChangeCuttingAbility();
                cursorVisibility.TurnCursorVisible();
                isCursorVisible = true;
                ReturnToPosition();
            }

            FollowMouse.ObjectFollowsMouse(rb);
        }
    }
    void ReturnToPosition()
    {
        gameObject.transform.position = startPos.position;
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
}
