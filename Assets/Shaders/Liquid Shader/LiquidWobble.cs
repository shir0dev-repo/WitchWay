using UnityEngine;

public class LiquidWobble : MonoBehaviour
{
    [Header("References")]
    private Renderer _renderer;
    [SerializeField] private int _materialIndex;

    [Header("Wobble Settings")]
    [SerializeField] private float _maxWobble = 0.03f;
    [SerializeField] private float _wobbleSpeed = 1.0f;
    [SerializeField] private float _recoveryScale = 1.0f;
    // wobble
    private float _wobbleAmountX = 0.0f, _wobbleAmountZ = 0.0f;
    private float _wobbleAmountToAddX = 0.0f, _wobbleAmountToAddZ = 0.0f;
    private float _pulse;

    // transform info
    private Vector3 _lastPos;
    private Vector3 _lastRot;
    private Vector3 _velocity;
    private Vector3 _wobbleVelocity = Vector3.zero;
    private Vector3 _velocityLastFrame;
    private Vector3 _angularVelocity;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _lastPos = transform.position;
        _lastRot = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        if (_renderer == null) return;

        _wobbleAmountToAddX = Mathf.Lerp(_wobbleAmountToAddX, 0, Time.deltaTime * (_recoveryScale));
        _wobbleAmountToAddZ = Mathf.Lerp(_wobbleAmountToAddZ, 0, Time.deltaTime * (_recoveryScale));

        // make a sine wave of the decreasing wobble
        _pulse = 2 * Mathf.PI * _wobbleSpeed;
        _wobbleAmountX = _wobbleAmountToAddX * Mathf.Sin(_pulse * Time.time);
        _wobbleAmountZ = _wobbleAmountToAddZ * Mathf.Sin(_pulse * Time.time);

        // send it to the shader
        _renderer.materials[_materialIndex].SetFloat("_WobbleX", _wobbleAmountX);
        _renderer.materials[_materialIndex].SetFloat("_WobbleZ", _wobbleAmountZ);

        // velocity
        _velocity = (_lastPos - transform.position) / Time.deltaTime;
        _angularVelocity = transform.rotation.eulerAngles - _lastRot;


        // add clamped velocity to wobble
        _wobbleAmountToAddX += Mathf.Clamp((_velocity.x + (_angularVelocity.z * 0.2f)) * _maxWobble, -_maxWobble, _maxWobble);
        _wobbleAmountToAddZ += Mathf.Clamp((_velocity.z + (_angularVelocity.x * 0.2f)) * _maxWobble, -_maxWobble, _maxWobble);

        // keep last position
        _lastPos = transform.position;
        _lastRot = transform.rotation.eulerAngles;
    }
}
