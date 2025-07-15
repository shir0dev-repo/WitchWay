using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StandardDeviation
{
    public readonly List<Vector3> Points;

    public int PointCount => Points.Count;

    public float Deviation => _deviation;
    [SerializeField] private float _deviation = 0.0f;

    public float Direction => _direction;
    [SerializeField] private float _direction = 0.0f;

    [Header("Parameters")]
    public float MaxDeviation => _maxDeviation;
    [SerializeField] private float _maxDeviation = 50.0f;

    public bool IsClockwiseCollection => _direction > 0.0f;

    public StandardDeviation()
    {
        Points = new List<Vector3>();
    }

    public void AddPoint(Vector3 point)
    {
        Points.Add(point);
    }

    public void RemovePoint(int index)
    {
        if (index < 0 || index >= Points.Count) return;

        Points.RemoveAt(index);
    }

    public void SetPoint(int index, Vector3 point)
    {
        if (index < 0 || index >= Points.Count) return;

        Points[index] = point;
    }

    public void SetPoints(IEnumerable<Vector3> points)
    {
        Points.Clear();
        Points.AddRange(points);
    }

    public void ClearPoints()
    {
        Points.Clear();
    }

    public float Recalculate()
    {
        if (Points.Count <= 2)
        {
            _deviation = 0.0f;
            _direction = 0.0f;
            return 0.0f;
        }

        Vector3 center = CalculateCenter(Points);
        _direction = 0.0f;
        List<float> radii = new();
        float sum = 0;
        float lastAngle = 0.0f;
        float deltaAngle = 0.0f;
        foreach (Vector3 point in Points)
        {
            Vector3 local = point - center;
            float r = local.magnitude;
            sum += r;
            radii.Add(r);

            float angle = Mathf.Atan2(local.y, local.x);
            float deltaD = Mathf.DeltaAngle(lastAngle * Mathf.Rad2Deg, angle * Mathf.Rad2Deg);

            deltaAngle += deltaD;
            lastAngle = angle;
        }

        float mean = sum / radii.Count;

        UpdateDeviation(radii, mean, _maxDeviation, deltaAngle);

        return Deviation;
    }

    private void UpdateDeviation(List<float> radii, float meanRadius, float maxDeviation, float deltaAngle)
    {
        _deviation = CalculateDeviation(radii, meanRadius, maxDeviation);
        _direction = deltaAngle < 0.0f ? -1 : 1;
    }

    /// <summary>Calculates the average of a list of points.</summary>
    public static Vector3 CalculateCenter(List<Vector3> points)
    {
        Vector3 center = Vector3.zero;
        float invCount = 1.0f / points.Count;
        foreach (Vector3 p in points)
        {
            center += p * invCount;
        }

        return center;
    }

    public static float CalculateDeviation(List<float> radii, float meanRadius, float maxDeviation = 1.0f)
    {
        float sumSqrDiffs = 0.0f;

        foreach (float radius in radii)
        {
            float diff = radius - meanRadius;
            sumSqrDiffs += diff * diff;
        }

        float std = Mathf.Sqrt(sumSqrDiffs / (radii.Count - 1));
        return 1 - Mathf.Clamp01(std / maxDeviation);
    }
}
