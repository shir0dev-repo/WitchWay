using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using FMODUnity;
using UnityEngine.UI;

public class CauldronController : Singleton<CauldronController>
{
    [SerializeField] private TextMeshProUGUI _stdDevUGUI;

    [Header("Mixing")]
    [SerializeField] private float _mixTimer = 5.0f;
    [SerializeField] private float _directionSwitchTimer = 4.0f;

    [Header("Deviation")]
    public StandardDeviation Deviation = new StandardDeviation();
    [SerializeField, Range(0, 1)] private float _deviationOKThreshold = 0.5f;
    [SerializeField, Range(1, 5)] private float _deviationHoldTime = 3.0f;
    [SerializeField, Range(5, 50)] private int _maxPointCount = 25;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer _circleUI;
    [SerializeField] private float _arrowRotateAccel = 2.5f;
    [SerializeField] private float _maxArrowRotateSpeed = 8.0f;
    private float _arrowRotateVelocity = 0.0f;

    private bool _isStirringCW = true;
    float TimeSpentStirring = 6f;
    private Vector3 _cursorPos = Vector3.zero;

    private float _addInterval = 0.1f;
    private float _addTimer = 0.0f;
    private float _holdTimer = 0.0f;

    float switchStirDirectionTimer = 4.0f;

    private float _progress = 0.0f;

    void Start()
    {
        switchStirDirectionTimer = _directionSwitchTimer;
        gameObject.SetActive(false);
        _circleUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _progress = 0.0f;
        _holdTimer = 0.0f;
        _isStirringCW = true;
        _circleUI.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _circleUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (CauldronMaster.Instance == null || !CauldronMaster.Instance.CurrentlyMixing) return;

        UpdateCursorPoints();

        if (Input.GetMouseButton(0))
        {
            // add cursor position to point list
            TryAddPoint();

            Deviation.Recalculate();
            
            bool withinDev = IsWithinDeviationThreshold(Deviation.Deviation);
            bool correctDirection = IsStirringCorrectDirection(Deviation.Direction, _isStirringCW);
            
            _stdDevUGUI.text =
            $"Deviation: {Deviation.Deviation:F2} " +
            "\nTarget: " + (_isStirringCW ? "CW" : "CCW") +
            "\nCurrent: " + (IsStirringCW(Deviation.Direction) ? "CW" : "CCW");

            if (!correctDirection)
            {
                UpdateStirringUI(false);
            }

            if (!withinDev)
            {
                UpdateStirringUI(correctDirection);
                _holdTimer = 0.0f;
            }

            if (!correctDirection || !withinDev)
            {
                _arrowRotateVelocity = Mathf.Clamp(_arrowRotateVelocity - _arrowRotateAccel * Time.deltaTime, 0.0f, _maxArrowRotateSpeed);
                return;
            }
            else
            {
                _arrowRotateVelocity = Mathf.Clamp(_arrowRotateVelocity + _arrowRotateAccel * Time.deltaTime, 0.0f, _maxArrowRotateSpeed);
                _progress += Time.deltaTime;
                UpdateStirringUI(true);
            }

            
            if (_progress >= _mixTimer)
            {
                GameEvents.Crafting.OnCauldronMixSequenceCompleted?.Invoke();
                gameObject.SetActive(false);
                _circleUI.gameObject.SetActive(false);
            }
            else
            {
                GameEvents.Crafting.OnCauldronMixProgressIncreased?.Invoke(_progress / _mixTimer);
            }

            // compare deviation to threshold
            _holdTimer += Time.deltaTime;
            switchStirDirectionTimer -= Time.deltaTime;
            
            if (switchStirDirectionTimer <= 0 || _holdTimer >= _deviationHoldTime)
            {
                ChangeStirringDirection();
                switchStirDirectionTimer = _directionSwitchTimer;
                _holdTimer = 0.0f;
            }
        }
        else // !GetMouseButton(0)
        {
            _arrowRotateVelocity = Mathf.Clamp(_arrowRotateVelocity - _arrowRotateAccel * Time.deltaTime, 0.0f, _maxArrowRotateSpeed);
            UpdateStirringUI(true);
        }
    }

    /// <summary>Updates the cursor position.</summary>
    private void UpdateCursorPoints()
    {
        if (Input.GetMouseButton(0))
        {
            _cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _addTimer = 0;
            Deviation.ClearPoints();
            Deviation.AddPoint(_cursorPos);
        }
    }

    /// <summary>Adds a point to <see cref="CauldronController._cursorPoints"/> if timer is ready.</summary>
    private void TryAddPoint()
    {
        _addTimer += Time.deltaTime;
        if (_addTimer >= _addInterval)
        {
            _addTimer -= _addInterval;
            Deviation.AddPoint(_cursorPos);
        }

        if (Deviation.PointCount > _maxPointCount)
        {
            Deviation.RemovePoint(0);
        }
    }

    /// <summary>Checks if current deviation is within threshold.</summary>
    private bool IsWithinDeviationThreshold(float deviation)
    {
        return Deviation.PointCount >= _maxPointCount / 2 && deviation >= _deviationOKThreshold;
    }

    private static bool IsStirringCW(float totalAngle)
    {
        return totalAngle < 0.0f;
    }

    /// <summary>Checks if stir direction is the same as target direction.</summary>
    private static bool IsStirringCorrectDirection(float totalAngle, bool isTargetCW)
    {
        return isTargetCW ? IsStirringCW(totalAngle) : !IsStirringCW(totalAngle);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;

        Vector3[] points = Deviation.Points.ToArray();
        Camera c = Camera.main;
        
        for (int i = 0; i < points.Length; i++)
        {
            points[i].z = -c.transform.position.z;
            points[i] = c.ScreenToWorldPoint(points[i]);
        }

        Gizmos.DrawLineStrip(points, true);
    }

    public void ChangeStirringDirection()
    {
        _isStirringCW = !_isStirringCW;
        GameEvents.Crafting.OnCauldronMixStepCompleted?.Invoke();
        SpriteRenderer sr = _circleUI.GetComponent<SpriteRenderer>();
        _arrowRotateVelocity = 0.0f;
        sr.flipX = !sr.flipX;
    }

    void StirringInWrongDirection()
    {
        
    }

    private void UpdateStirringUI(bool stirringCorrectDirection)
    {
        float speed = Mathf.Clamp(_arrowRotateVelocity, 0.0f, _maxArrowRotateSpeed);
        _circleUI.transform.Rotate(0, 0, _circleUI.flipX ? speed : -speed);
    }
}
