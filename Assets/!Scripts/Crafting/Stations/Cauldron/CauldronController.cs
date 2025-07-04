using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CauldronController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _stdDevUGUI;

    RecipeSO recipe;

    [Header("Mixing")]
    [SerializeField] private float _mixTimer = 5.0f;

    [Header("Deviation")]
    public StandardDeviation Deviation = new StandardDeviation();
    [SerializeField, Range(0, 1)] private float _deviationOKThreshold = 0.5f;
    [SerializeField, Range(1, 5)] private float _deviationHoldTime = 3.0f;
    [SerializeField, Range(5, 50)] private int _maxPointCount = 25;

    private bool _isStirringCW = true;
    float TimeSpentStirring = 6f;
    private Vector3 _cursorPos = Vector3.zero;
    //private List<Vector3> _cursorPoints = new();

    private float _addInterval = 0.1f;
    private float _addTimer = 0.0f;
    private float _holdTimer = 0.0f;

    float switchStirDirectionTimer = 10.0f;

    private float _progress = 0.0f;

    void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateCursorPoints();

        if (Input.GetMouseButton(0))
        {
            // add cursor position to point list
            TryAddPoint();

            Deviation.Recalculate();
            
            bool withinDev = IsWithinDeviationThreshold(Deviation.Deviation);
            bool correctDirection = IsStirringCorrectDirection(Deviation.Direction, _isStirringCW);

            if (!correctDirection)
            {
                StirringInWrongDirection();      
                return;
            }

            CauldronMaster.Instance.Duration.UpdateCurrentDuration();

            if (!withinDev)
            {
                _holdTimer = 0.0f;
                return;
            }

            _progress += Time.deltaTime;
            if (_progress >= _mixTimer)
            {
                GameEvents.Crafting.OnCauldronMixSequenceCompleted?.Invoke();
            }

            // compare deviation to threshold
            _holdTimer += Time.deltaTime;
            if (_holdTimer >= _deviationHoldTime)
            {
                ChangeStirringDirection();
            }

            switchStirDirectionTimer -= Time.deltaTime;
            if (switchStirDirectionTimer <= 0)
            {
                ChangeStirringDirection();
                switchStirDirectionTimer = 10.0f;
                // simple timer function for switching directions
            }

            _stdDevUGUI.text =
            $"Deviation: {Deviation.Deviation:F2} " +
            "\nTarget: " + (_isStirringCW ? "CW" : "CCW") +
            "\nCurrent: " + (IsStirringCW(Deviation.Direction) ? "CW" : "CCW");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeStirringDirection();
            // simple testing function, only works when the mouse is held down
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
    }

    void StirringInWrongDirection()
    {
        TimeSpentStirring -= Time.deltaTime;

        if (TimeSpentStirring <= 0)
        {
            TimeSpentStirring = 6f;
            Debug.Log("you've been stirring the wrong way for a while...");
        }
    }
}
