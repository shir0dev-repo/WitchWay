using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CuttableIngredient : MonoBehaviour
{
    
    [SerializeField] Transform[] _cutPoints;
    [SerializeField] private int _dstThreshold = 50;

    List<Vector3> _cursorPoints = new List<Vector3>();
    Vector3 _cursorPos = Vector3.zero;
    CuttingBoard _board;

    private WorldIngredient _ingredient;

    //public IngredientSO _ingredientSO;
    // when the cuttable is instantiated, this should already be filled (hopefully)

    int _cutCount = 0;
    float ingredientDurability = 100f;
    private float _cutInterval = 0.25f, _cutTimer = 0.25f;

    private bool _isCutting = false;

    Camera _mainCamera;

    private void Start()
    {
        _board = CuttingBoard.Instance;
        _mainCamera = Camera.main;
        _ingredient = GetComponent<WorldIngredient>();
    }

    private void Update()
    {
        if (!_board.CanCut)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CompleteChopping();
            }
            return;
        }

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

        foreach (Transform t in _cutPoints)
        {
            float xPosition = _mainCamera.WorldToScreenPoint(t.position).x;

            if (_cursorPoints.Any(p => Mathf.Abs(p.x - xPosition) > _dstThreshold))
                continue;
            
            else
            {
                success = true;
                break;
            }
        }

        if (success)
        {
            Debug.Log("Yay!");
            UpdateChoppingProgress(true);
            // on a successful cut, add to the count
        }
        else { UpdateChoppingProgress(false); }
    }
    void UpdateChoppingProgress(bool result)
    {
        _cutCount++;
        ingredientDurability = Mathf.Clamp(ingredientDurability - 10f, 0f, 100f);

        CheckIngredientStatus();
    }
    void CompleteChopping()
    {
        _ingredient.ModifiedState.Cut();

        Debug.Log("player is done cutting!" + '\n' + RateChopping());
        // later, this will just grab the name of the scriptable object attached to the prefab.
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
            }
            else
            {
                Debug.Log("the ingredient has been rendered unuseable...");
                // will add more to this later
            }
        }
    }
}
