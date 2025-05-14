using UnityEngine;

public class Pestle : MonoBehaviour
{
    InvisibleCursor cursorVisibility;
    FollowMouse FollowMouse;
    Rigidbody rb;

    Vector3 startPos;

    bool isCursorVisible = true;
    void Start()
    {
        cursorVisibility = GetComponent<InvisibleCursor>(); //add this to the pestle so it can turn off the cursor
        FollowMouse = GetComponent<FollowMouse>();
        rb = GetComponent<Rigidbody>(); 

        startPos = gameObject.transform.position;
    }

    void Update()
    {
        if (isCursorVisible)
        {
            if (Input.GetMouseButtonDown(0) && CastRay())
            {
                cursorVisibility.TurnCursorInsivible();
                isCursorVisible = false;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                cursorVisibility.TurnCursorVisible();
                isCursorVisible = true;
                ReturnToPosition();
            }

            FollowMouse.ObjectFollowsMouse(rb);
        }
    }

    void ReturnToPosition()
    {
        gameObject.transform.position = startPos;
        gameObject.transform.rotation = Quaternion.identity;
        rb.linearVelocity = Vector3.zero;
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
