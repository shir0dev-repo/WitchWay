using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CauldronUI : MonoBehaviour
{
    [SerializeField] private Transform _centerPoint;
    [SerializeField] private TextMeshProUGUI _stdDevUGUI;

    RecipeSO recipe;

    [Header("Deviation")]
    [SerializeField, Range(5, 100)] private float _maxDeviation = 50;
    [SerializeField, Range(0, 1)] private float _deviationOKThreshold = 0.5f;
    [SerializeField, Range(1, 5)] private float _deviationHoldTime = 3.0f;
    [SerializeField, Range(5, 50)] private int _pointCount = 50;

    [Header("Mixing")]
    [SerializeField] private float _totalMixingDuration = 15.0f;

    private bool _isStirringCW = true;
    float TimeSpentStirring = 6f;
    private Vector3 _cursorPos = Vector3.zero;
    private List<Vector3> _cursorPoints = new();

    private float _addInterval = 0.1f;
    private float _addTimer = 0.0f;
    private float _holdTimer = 0.0f;

    float switchStirDirectionTimer = 10.0f;

    private float _progress = 0.0f;

    List<WorldIngredient> ingredients;
    public void Enable(int stationID)
    {
        gameObject.SetActive(stationID == 3);
    }

    [ContextMenu("Validate")]
    public void Finish()
    {
        ingredients = CauldronMaster.Instance.InsidePot.GetIngredients();
        recipe = RecipeBook.Instance.list.GetFirstRecipeFromListofMultiple(ingredients);
        if (recipe == null) { return; }

        Debug.Log("the closest recipe to the ingredients in the pot is " + recipe.ToString());

        if (recipe.IsValidRecipe(ingredients.Select(ing => ing.ModifiedState).ToList()))
        {
            if (recipe.IsDiscovered)
            {
                Debug.Log("win epic!" + '\n' + "the outputted potion is: " + recipe.Output.ToString());
            }
            else
            {
                PotionData mysterious = RecipeBook.Instance.MysteriousPotion;
                Debug.Log("you haven't discovered this recipe yet!" + '\n' + "the outputted potion is: " + mysterious.ToString());
            }

            CauldronMaster.Instance.InsidePot.UseIngredientsInValidRecipe();
        }
        else
        {
            Debug.Log("NOOOOOOOOOOOOOOOOOOO");
            CauldronMaster.Instance.InsidePot.ReturnRejectedIngredients();
        }
    }

    void Start()
    {
        CauldronEvents.ActivateMixing += ActivateMixing;
        CauldronEvents.ActivateMixing += SetIngredients;

        CauldronEvents.DeactivateMixing += ActivateMixing;
        CauldronEvents.DeactivateMixing += Finish;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateCursorPoints();

        if (Input.GetMouseButton(0))
        {
            // add cursor position to point list
            TryAddPoint();

            float deviation = CalculateCircleAccuracy(out float totalAngle);
            bool withinDev = IsWithinDeviationThreshold(deviation);
            bool correctDirection = IsStirringCorrectDirection(totalAngle, _isStirringCW);

            if (!correctDirection)
            {
                StirringInWrongDirection();
                return;
            }

            if (!withinDev)
            {
                _holdTimer = 0.0f;
                return;
            }

            // compare deviation to threshold

            _holdTimer += Time.deltaTime;
            if (_holdTimer >= _deviationHoldTime)
            {
                ChangeStirringDirection();
            }


            _progress += Time.deltaTime;
            if (_progress >= _totalMixingDuration)
            {
                Finish();
                return;
            }

            switchStirDirectionTimer -= Time.deltaTime;
            if (switchStirDirectionTimer <= 0)
            {
                ChangeStirringDirection();
                switchStirDirectionTimer = 10.0f;
                // simple timer function for switching directions
            }
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
            _cursorPoints.Clear();
            _cursorPoints.Add(_cursorPos);
        }
    }

    /// <summary>Adds a point to <see cref="CauldronUI._cursorPoints"/> if timer is ready.</summary>
    private void TryAddPoint()
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
    }

    /// <summary>Checks if current deviation is within threshold.</summary>
    private bool IsWithinDeviationThreshold(float deviation)
    {
        return _cursorPoints.Count >= _pointCount / 2 && deviation >= _deviationOKThreshold;
    }

    private float CalculateCircleAccuracy(out float deltaAngle)
    {
        Vector3 center = CalculateCenter(_cursorPoints);
        List<float> radii = new();
        float sum = 0;
        deltaAngle = 0.0f;
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

    /// <summary>Checks if stir direction is the same as target direction.</summary>
    private static bool IsStirringCorrectDirection(float totalAngle, bool isTargetCW)
    {
        return isTargetCW ? IsStirringCW(totalAngle) : !IsStirringCW(totalAngle);
    }

    /// <summary>Calculates the average of a list of points.</summary>
    private static Vector3 CalculateCenter(List<Vector3> points)
    {
        Vector3 center = Vector3.zero;
        float invCount = 1.0f / points.Count;
        foreach (Vector3 p in points)
        {
            center += p * invCount;
        }

        return center;
    }

    /// <summary>Calculates the standard deviation of a list of points.</summary>
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
        for (int i = 0; i < points.Length; i++)
        {
            points[i].z = 10;
            points[i] = c.ScreenToWorldPoint(points[i]);
        }

        Gizmos.DrawLineStrip(points, true);
    }

    void ActivateMixing()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    void SetIngredients()
    {
        ingredients = CauldronMaster.Instance.InsidePot.GetIngredients();
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
