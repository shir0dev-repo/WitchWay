using UnityEngine;

[System.Serializable]
public class TemperatureTarget
{
    public float TimeSpentAtTarget { get; private set; } = 0f;
    public float TargetTemperature { get; private set; } = 0f;
    private float _allowedDeviance = 0.0f;
    public TemperatureTarget(float initialTarget, float allowedDeviance)
    {
        TargetTemperature = initialTarget;
        _allowedDeviance = allowedDeviance;
    }

    public void Update(float currentTemperature)
    {
        if (IsWithinDeviance(currentTemperature))
        {
            TimeSpentAtTarget += Time.deltaTime;
        }
    }

    public void SetTarget(float target)
    {
        TargetTemperature = target;
    }

    public bool IsWithinDeviance(float temperature)
    {
        float dst = Mathf.Abs(temperature - TargetTemperature);
        float high = _allowedDeviance * 0.5f;
        float low = -high;

        return dst >= low && dst <= high;
    }
}
