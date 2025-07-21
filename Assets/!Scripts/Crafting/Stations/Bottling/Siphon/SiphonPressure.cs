using UnityEngine;

[System.Serializable]
public class SiphonPressure
{
    private const float HALF_PI = Mathf.PI * 0.5f;

    [Header("Fall Speed")]
    public float MinFallSpeed => _minFallSpeed;
    [SerializeField] private float _minFallSpeed = 5.0f;
    
    public float MaxFallSpeed => _maxFallSpeed;
    [SerializeField] private float _maxFallSpeed = 25.0f;
    
    public float MinValue => _minValue;
    private float _minValue;
    
    public float MaxValue => _maxValue;
    private float _maxValue;

    public float Increment => _increment;
    [SerializeField] private float _increment = 2.5f;

    public float Value => _currentValue;
    private float _currentValue = 0.0f;

    private float _targetValue = 0.0f;
    private float _timeSinceLastIncrease = 0.0f;

    public SiphonPressure(float minPressure, float maxPressure)
    {
        _minValue = minPressure;
        _maxValue = maxPressure;
    }

    public void Reset()
    {

    }

    public void Update(float deltaTime)
    {
        _timeSinceLastIncrease += deltaTime;
    }

    public void Increase()
    {
        _timeSinceLastIncrease = 0.0f;
        _targetValue = Mathf.Clamp(_targetValue + _increment, _minValue, _maxValue);
    }

    public void Decrease()
    {
        float currentPercent = Mathf.Clamp01(1.0f - (_targetValue * 0.01f));
        float decrease = Mathf.Abs(Mathf.Lerp(_maxFallSpeed, _minFallSpeed, currentPercent));

        _targetValue = Mathf.Clamp(_targetValue - decrease * Time.deltaTime, _minValue, _maxValue);
    }

    public void DoEase()
    {
        float easeProgress = Mathf.Clamp01(_timeSinceLastIncrease * 2.0f);
        float value = Mathf.Sin(easeProgress * HALF_PI);

        _currentValue = Mathf.Lerp(_currentValue, _targetValue, value);
    }
}
