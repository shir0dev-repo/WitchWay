using UnityEngine;

public class EmptyBottle : MonoBehaviour
{
    FollowMouse mouse;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mouse = GetComponent<FollowMouse>();
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            mouse.ObjectFollowsMouse(rb);
        }
    }
}
