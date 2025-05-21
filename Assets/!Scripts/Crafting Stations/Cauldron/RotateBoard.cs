using UnityEngine;

public class RotateBoard : MonoBehaviour
{
    public Transform orb;
    float radius = 0.5f;

    Transform pivot;

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
            Vector3 orbVector = Camera.main.WorldToScreenPoint(orb.position);
            orbVector = Input.mousePosition - orbVector;
            float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;

            pivot.position = orb.position;
            pivot.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
        
    }
}
