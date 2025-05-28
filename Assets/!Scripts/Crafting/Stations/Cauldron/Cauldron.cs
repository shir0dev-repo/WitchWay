using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    [SerializeField] private Transform _centerPoint;
    [SerializeField] private TextMeshProUGUI _stdDevUGUI;
    
    [Header("Deviation")]
    [SerializeField, Range(5, 100)] private float _maxDeviation = 50;
    [SerializeField, Range(0, 1)] private float _deviationOKThreshold = 0.5f;
    [SerializeField, Range(1, 5)] private float _deviationHoldTime = 3.0f;
    [SerializeField, Range(5, 50)] private int _pointCount = 50;

    private bool _isStirringCW = true;
    private Vector3 _cursorPos = Vector3.zero;
    private List<Vector3> _cursorPoints = new();

    private float _addInterval = 0.1f;
    private float _addTimer = 0.0f;
    private float _holdTimer = 0.0f;

    void Start()
    {
        SwitchToMixing.mixingMode += ActivateMixing;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            _cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _addTimer = 0;
            _cursorPoints.Clear();
            _cursorPoints.Add(_cursorPos);
        }

        if (Input.GetMouseButton(0))
        {
            _addTimer += Time.deltaTime;
            if (_addTimer >= _addInterval)
            {
                _addTimer -= _addInterval;
                _cursorPoints.Add(_cursorPos);
            }

            if (_cursorPoints.Count > _pointCount)
            {
                _cursorPoints.RemoveAt(0);
            }

            float deviation = CalculateCircleAccuracy();
            if (_cursorPoints.Count >= _pointCount / 2 && deviation >= _deviationOKThreshold)
            {
                _holdTimer += Time.deltaTime;
                if (_holdTimer >= _deviationHoldTime)
                {
                    _isStirringCW = !_isStirringCW;
                }
            }
            else
            {
                _holdTimer = 0.0f;
            }
        }
    }

    private float CalculateCircleAccuracy()
    {
        Vector3 center = Camera.main.WorldToScreenPoint(_centerPoint.position);
        List<float> radii = new();
        float sum = 0;
        float deltaAngle = 0.0f;
        float lastAngle = 0.0f;

        foreach (Vector3 p in _cursorPoints)
        {
            Vector3 offset = p - center;
            float r = offset.magnitude;
            sum += r;
            radii.Add(r);


            float angle = Mathf.Atan2(offset.y, offset.x);
            float deltaD = Mathf.DeltaAngle(lastAngle * Mathf.Rad2Deg, angle * Mathf.Rad2Deg);
            
            deltaAngle += deltaD;
            lastAngle = angle;
        }

        float mean = sum / radii.Count;
        
        float std = StandardDeviation(radii, mean, deltaAngle, _maxDeviation, _isStirringCW);
        _stdDevUGUI.text = 
            $"Deviation: {std:F2} " +
            "\nTarget: " + (_isStirringCW ? "CW" : "CCW") +
            "\nCurrent: " + (IsStirringCW(deltaAngle) ? "CW" : "CCW");
        return std;
    }

    private static bool IsStirringCW(float totalAngle)
    {
        return totalAngle < 0.0f;
    }

    private static bool IsStirringCorrectDirection(float totalAngle, bool isTargetCW)
    {
        return (isTargetCW) ? IsStirringCW(totalAngle) : !IsStirringCW(totalAngle);
    }

    private static float StandardDeviation(List<float> radii, float mean, float totalAngle, float maxDeviation, bool isTargetCW)
    {
        float sumSqrDiffs = 0.0f;

        foreach (float v in radii)
        {
            float diff = v - mean;
            sumSqrDiffs += diff * diff;
        }

        float std = Mathf.Sqrt(sumSqrDiffs / (radii.Count - 1));
        if (IsStirringCorrectDirection(totalAngle, isTargetCW))
            return 1 - Mathf.Clamp01(std / maxDeviation);
        else return 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3[] points = _cursorPoints.ToArray();
        Camera c = Camera.main;
        for(int i = 0; i < points.Length; i++)
        {
            points[i].z = 10;
            points[i] = c.ScreenToWorldPoint(points[i]);
        }

        Gizmos.DrawLineStrip(points, true);
    }

    void ActivateMixing()
    {
        gameObject.SetActive(true);
    }
}
