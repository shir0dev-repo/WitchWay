using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    public bool IsMoving { get; private set; }

    [SerializeField] private Volume _blurVolume;
    [Space]
    [SerializeField] private float _speed = 4.0f;

    private Vector3 _lastPosition = Vector3.zero;
    private Vector3 _targetPosition = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        _targetPosition = transform.position;
        _lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (!IsMoving) return;

        transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, _speed * Time.deltaTime);
        
        float lerp = Mathf.InverseLerp(_lastPosition.x, _targetPosition.x, transform.position.x);
        float weight = Mathf.Sin(Mathf.PI * lerp);
        _blurVolume.weight = weight;

        if (_velocity.sqrMagnitude < 0.1f * 0.1f)
        {
            _blurVolume.weight = 0.0f;
            IsMoving = false;
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        _lastPosition = _targetPosition;
        _targetPosition = position;
        IsMoving = true;
    }
}
