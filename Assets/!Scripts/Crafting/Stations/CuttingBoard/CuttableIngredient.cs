using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CuttableIngredient : MonoBehaviour
{
    [SerializeField] private int _dstThreshold = 50;

    [Space]
    [SerializeField] Transform[] _cutPoints;

    [Header("Segments")]
    [SerializeField] private List<IngredientSegment> _segments = new();
    [SerializeField] private float _grabVelocity = 2.5f;
    [SerializeField] private float _maxGrabVelocity = 1.5f;

    List<Vector3> _cursorPoints = new List<Vector3>();
    Vector3 _cursorPos = Vector3.zero;
    CuttingBoard _board;

    private WorldIngredient _ingredient;

    int _cutCount = 0;
    float ingredientDurability = 100f;
    private float _cutInterval = 0.25f, _cutTimer = 0.25f;

    private bool _isCutting = false;

    Camera _mainCamera;

    static event Action EndAction;

    private void Start()
    {
        _board = CuttingBoard.Instance;
        _mainCamera = Camera.main;
        _ingredient = GetComponent<WorldIngredient>();

        GameEvents.Crafting.OnToolDeselected += CompleteChopping;

        foreach (IngredientSegment segment in _segments)
        {
            segment.GrabVelocity = _grabVelocity;
            segment.MaxGrabVelocity = _maxGrabVelocity;
        }

        EndAction = _ingredient.ModifiedState.Cut;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (IngredientSegment segment in _segments)
        {
            segment.GrabVelocity = _grabVelocity;
            segment.MaxGrabVelocity = _maxGrabVelocity;
        }
    }
#endif

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _cursorPoints.Clear();
            _isCutting = true;
            _cutTimer = 0;
        }

        if (Input.GetMouseButton(0))
        {
            _cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isCutting = false;
            _cutTimer = _cutInterval;
            CompareCuts();
        }

        if (!_isCutting) return;

        _cutTimer -= Time.deltaTime;
        if (_cutTimer <= 0.0f)
        {
            if (_cursorPoints.Count == 0 || Vector3.Distance(_cursorPos, _cursorPoints[^1]) > _dstThreshold)
            {
                _cutTimer = _cutInterval;
                _cursorPoints.Add(_cursorPos);
            }
        }
    }

    private void CompareCuts()
    {
        bool success = false;

        if (_cursorPoints.Count < 2) return;
        if (IsCutUpright() == false) return;
        
        Transform targetCutPoint = null;

        foreach (Transform t in _cutPoints)
        {
            float xPosition = _mainCamera.WorldToScreenPoint(t.position).x;

            if (_cursorPoints.Any(p => Mathf.Abs(p.x - xPosition) > _dstThreshold))
                continue;

            else
            {
                success = true;
                targetCutPoint = t;
                break;
            }
        }

        if (success && targetCutPoint != null)
        {
            Debug.Log("Yay!");
            GameEvents.Crafting.OnCutItem?.Invoke(_ingredient, targetCutPoint);
            TryDetachSegment(targetCutPoint);
            // on a successful cut, add to the count
        }

        UpdateChoppingProgress();
    }

    private void TryDetachSegment(Transform targetCutPoint)
    {
        if (TryGetLastTwoSegments().Length == 2 && != null)
        {
            var lastTwo = TryGetLastTwoSegments();
            ChopLastTwoSegmentsLeft(lastTwo[0], lastTwo[1]);

            return; 
        }

        var closestPair = GetClosestPairToCutPoint(targetCutPoint.position);

        if (!closestPair.left.HasBeenDetached)
        {
            var leftSegments = _segments.Where(seg => seg.Center.x < closestPair.left.Center.x);
            if (!leftSegments.Any())
            {
                closestPair.left.Detach();
            }
            else
            {
                foreach (var segment in leftSegments)
                {
                    if (segment.HasBeenDetached)
                    {
                        closestPair.left.Detach();
                        break;
                    }
                }
            }
        }

        if (!closestPair.right.HasBeenDetached)
        {
            var rightSegments = _segments.Where(seg => seg.Center.x > closestPair.right.Center.x);
            if (!rightSegments.Any())
            {
                closestPair.right.Detach();
            }
            else
            {
                foreach (var segment in rightSegments)
                {
                    if (segment.HasBeenDetached)
                    {
                        closestPair.right.Detach();
                        break;
                    }
                }
            }
        }
    }
    private (IngredientSegment left, IngredientSegment right) GetClosestPairToCutPoint(Vector3 cutPoint)
    {
        // sort by distance to cut point
        // order from left-right
        // take the first 2 pieces (left and right piece of cutpoint)
        IngredientSegment[] closestPair =
            _segments.OrderBy(seg => Vector3.Distance(cutPoint, seg.Center))
                //.ThenBy(seg => seg.Center.x)
                .Take(2)
                .ToArray();

        return (closestPair[0], closestPair[1]);
    }
    IngredientSegment[] TryGetLastTwoSegments()
    {
        if (_segments.Count == 0) return null;
        IngredientSegment[] lastSegments = _segments.Where(i => !i.HasBeenDetached).ToArray();

        return lastSegments;
    }
    void ChopLastTwoSegmentsLeft(IngredientSegment one, IngredientSegment two)
    {
        one.Detach(); two.Detach();
    }
    void UpdateChoppingProgress()
    {
        _cutCount++;
        ingredientDurability = Mathf.Clamp(ingredientDurability - 10f, 0f, 100f);

        CheckIngredientStatus();
    }
    void CompleteChopping(ToolType type)
    {
        if (type == ToolType.Knife) EndAction?.Invoke();
        if (!IsAllSegmentsDetached()) { return; }

        Debug.Log("player is done cutting!" + '\n' + RateChopping());
    }
    string RateChopping()
    {
        if (_cutCount == 0) return "no cuts were made.";
        if (_cutCount > _cutPoints.Count()) return "you cut it too much!";
        if (_cutCount == _cutPoints.Count()) return "you cut it perfectly!";

        return "you cut it too little.";
    }
    void CheckIngredientStatus()
    {
        if (ingredientDurability < 10)
        {
            if (_ingredient.BaseIngredient.CanBeCrushed)
            {
                Debug.Log("the ingredient has been turned into a powder btw...");
                EndAction = _ingredient.ModifiedState.Crush;
            }
            else
            {
                Debug.Log("the ingredient has been rendered unuseable...");
                EndAction = DeleteIngredient;
            }
        }
    }
    bool IsCutUpright()
    {
        float top, bottom;

        top = _cursorPoints.First().y;
        bottom = _cursorPoints.Last().y;

        // Debug.Log("Top: "+top + " " + "Bottom: "+ bottom);
        // for debugging

        if (top > bottom) { return true; }
        else { return false; }
    }
    bool IsAllSegmentsDetached()
    {
        return _segments.All(x => x.HasBeenDetached); 
    }
    void DeleteIngredient()
    {
        Destroy(gameObject);
    }
}
