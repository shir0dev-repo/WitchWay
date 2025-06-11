using System.Collections;
using UnityEngine;

public class Stream : MonoBehaviour 
{
    [SerializeField] private float _maxRayDst = 2.0f;

    private LineRenderer _lineRenderer = null;
    [SerializeField] private Vector3 _targetPosition = Vector3.zero;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        MoveToPosition(0, transform.position);
        MoveToPosition(1, Vector3.zero);
    }

    public void Begin()
    {
        StartCoroutine(BeginPour());
    }

    private Vector3 FindEndPoint()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        Physics.Raycast(ray, out RaycastHit hit, _maxRayDst);

        Vector3 hitPoint = hit.collider ? hit.point : ray.GetPoint(_maxRayDst);
        return hitPoint;
    }

    private void MoveToPosition(int index, Vector3 targetPosition)
    {
        _lineRenderer.SetPosition(index, targetPosition);
    }

    private IEnumerator BeginPour()
    {
        while (gameObject.activeSelf)
        {
            _targetPosition = FindEndPoint();

            MoveToPosition(0, transform.position);
            MoveToPosition(1, _targetPosition);

            yield return null;
        }

    }
}
