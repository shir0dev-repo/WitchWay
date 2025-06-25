using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField]
    Transform TargetLocation;
    Transform Prevlocation;

    void Start()
    {
        Prevlocation = gameObject.transform;

        CauldronEvents.ActivateMixing += ChangeLocation;
        CauldronEvents.DeactivateMixing += ReturnToLocation;
    }

    void ChangeLocation()
    {
        Camera.main.transform.position = TargetLocation.position;
        Camera.main.transform.rotation = TargetLocation.rotation;
    }
    void ReturnToLocation()
    {
        Camera.main.transform.position = new Vector3(0,5,-5);
        Camera.main.transform.rotation = new Quaternion(0.279829115f, 0, 0, 0.960049868f); 
    }

    //i'll eventually switch to using the camera manager just hold on
}
