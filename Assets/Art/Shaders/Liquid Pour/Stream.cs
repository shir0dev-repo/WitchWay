using System.Collections;
using UnityEngine;

public class Stream : MonoBehaviour 
{
    [Header("Line Settings")]
    [SerializeField] private float _maxRayDst = 2.0f;
    [SerializeField, Min(2)] private int _vertexCount = 2;
    
    [SerializeField] private GameObject _splashPS;

    [Header("Sine Ripple")]
    [SerializeField] private bool _shouldRipple = true;
    [SerializeField] private float _amplitude = 1.0f;
    [SerializeField] private float _frequency = 2.0f;
    private LineRenderer _lineRenderer = null;
    private Vector3 _targetPosition = Vector3.zero;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        SetupLineRenderer();
    }

    private void OnValidate()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _vertexCount;
    }

    private void SetupLineRenderer()
    {
        _lineRenderer.positionCount = _vertexCount;
        for (int i = 0; i < _vertexCount; i++)
        {
            MoveToPosition(i, transform.position);
        }
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

    private void MoveSplash(Vector3 location)
    {
        _splashPS.transform.position = location;
    }

    private IEnumerator BeginPour()
    {
        _splashPS.SetActive(true);

        while (gameObject.activeSelf)
        {
            _targetPosition = FindEndPoint();
            
            for (int i = 0; i < _vertexCount; i++)
            {
                float progress = (float)i / _vertexCount;
                MoveToPosition(i, EvaluateSine(transform.position, _targetPosition, progress));
            }

            MoveSplash(_targetPosition);

            yield return null;
        }

        _splashPS.SetActive(false);
    }

    private Vector3 EvaluateSine(Vector3 a, Vector3 b, float percent)
    {
        Vector3 lerp = Vector3.Slerp(a, b, percent);
        float sine = _amplitude * Mathf.Sin(Time.time * _frequency * Mathf.Sin(Mathf.PI * percent));
        return lerp;// new Vector3(lerp.x + sine, lerp.y, 0);
    }
}
