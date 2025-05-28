using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField]
    Transform TargetLocation;

    void Start()
    {
        SwitchToMixing.mixingMode += ChangeLocation;
    }

    void ChangeLocation()
    {
        Camera.main.transform.position = TargetLocation.position;
        Camera.main.transform.rotation = TargetLocation.rotation;
    }
}
