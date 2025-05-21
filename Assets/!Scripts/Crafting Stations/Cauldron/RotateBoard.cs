using UnityEngine;

public class RotateBoard : MonoBehaviour
{
    public Transform orb;
    float radius = 0.5f;
    float rotateSpd = 5;

    Transform pivot;
    float targetX;
    float currVelocity;

    void Start()
    {
        pivot = orb.transform;
        transform.parent = pivot;
        transform.position += Vector3.up * radius;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseY = Input.GetAxis("Mouse Y");
            targetX -= mouseY * rotateSpd;
            // just get the y of the mouse, no need for the rest of it
        }

        Vector3 currRotate = pivot.localEulerAngles;
        float newX = Mathf.SmoothDamp(currRotate.x, targetX, ref currVelocity, 0.1f);
        float clampedX = Mathf.Clamp(newX, 0, 40);
        // dampen the movement so it's not crazy, rotation is clamped so player cannot do crazy stuff

        pivot.localEulerAngles = new Vector3(clampedX, currRotate.y, currRotate.z);
    }
}
