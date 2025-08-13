using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    public bool IsMoving { get; private set; }

    [SerializeField] private Volume _blurVolume;
    [Space]
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private Transform _stationParent;

    public float defaultFOV = 60;
    float zoomSpeed = 2f;
    public Vector3 GetTargetPosition() => _targetPosition;
    private Vector3 _targetPosition = Vector3.zero;

    private Vector3 _lastPosition = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;

    [SerializeField] private ParallaxLayer[] parallaxLayers;
    private Vector3 _previousStationPos;


    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (Instance == null)
            Instance = this;

        _targetPosition = _stationParent.position;
        _lastPosition = _stationParent.position;
        _previousStationPos = _stationParent.position;
    }

    private void LateUpdate()
    {
        if (!IsMoving) return;

        _stationParent.position = Vector3.SmoothDamp(_stationParent.position, _targetPosition, ref _velocity, _speed * Time.deltaTime);

        float lerp = Mathf.InverseLerp(_lastPosition.x, _targetPosition.x, _stationParent.position.x);
        float weight = Mathf.Sin(Mathf.PI * lerp);
        _blurVolume.weight = weight;

        if (_velocity.sqrMagnitude < 0.1f * 0.1f)
        {
            _blurVolume.weight = 0.0f;
            IsMoving = false;
        }

        Vector3 delta = _stationParent.position - _previousStationPos;

        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.Move(delta);
        }

        _previousStationPos = _stationParent.position;
    }

    public void MoveToPosition(Vector3 position)
    {
        _lastPosition = _targetPosition;
        _targetPosition = position;
        IsMoving = true;
    }

    public void SetPosition(Vector3 position)
    {
        _stationParent.position = position;

        Vector3 delta = _stationParent.position - _previousStationPos;

        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.Move(delta);
        }

        _previousStationPos = _stationParent.position;
    }

    public void ZoomIn(float target) // remember! 60 is the default FOV!
    {
        Camera.main.fieldOfView = target;
    }
    public void ResetZoom()
    {
        Camera.main.fieldOfView = defaultFOV;
    }
}
