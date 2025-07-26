using UnityEngine;

//needs a limiter for how far from player
public class FishingAreaPositionIndicator : MonoBehaviour
{
    [Header("Sphere Settings")]
    [SerializeField] private GameObject fishingSpherePrefab;
    [SerializeField] private float sphereYOffset = -0.5f;
    [SerializeField] private float margin = 0.1f;

    private Plane fishingPlane;
    private Collider planeCollider; //in future could use edge collider to define area
    private GameObject spawnedSphere;

    private bool isFollowing = false;

    private Vector3 position;

    void Awake()
    {
        fishingPlane = new Plane(transform.up, transform.position);
        planeCollider = gameObject.GetComponent<Collider>();
    }

    public void StartFollowing()
    {
        if (spawnedSphere == null) spawnedSphere = Instantiate(fishingSpherePrefab, transform);

        isFollowing = true;
    }

    void Update()
    {
        if (!isFollowing || spawnedSphere == null) return;

        Ray lookRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (fishingPlane.Raycast(lookRay, out float enter))
        {
            Vector3 lookPoint = lookRay.GetPoint(enter);
            Vector3 closestPoint = planeCollider.ClosestPoint(lookPoint);

            Vector3 finalPos = new Vector3(closestPoint.x, transform.position.y + sphereYOffset, closestPoint.z);
            spawnedSphere.transform.position = finalPos;
            position = finalPos;
        }
    }

    public void StopFollowing()
    {
        isFollowing = false;
        Destroy(spawnedSphere);
    }

    public Vector3 GetPosition()
    {
        return position;
    }
}
