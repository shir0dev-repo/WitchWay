using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    public float CameraSpeed = 5.0f;
    public float Distance = 10.0f;
    public Transform CameraTarget;

    private Vector3 velocity = Vector3.zero;
    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, transform.position + transform.right, ref velocity, Time.deltaTime, CameraSpeed);
        transform.LookAt(CameraTarget);
    }
}
