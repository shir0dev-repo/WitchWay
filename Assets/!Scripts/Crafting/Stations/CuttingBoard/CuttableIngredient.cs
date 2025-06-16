using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CuttableIngredient : MonoBehaviour
{
    [SerializeField] private TrailRenderer _cursorTrail;

    [SerializeField] Transform[] _cutPoints;
    int _cutCount = 0;
    int _successfulCutCount = 0;
    float ingredientDurability;
    // get this from the ingredient's stats

    CuttingBoard _board = null;

    List<Vector3> _cursorPoints = new List<Vector3>();
    Vector3 _cursorPos = Vector3.zero;
    
    private bool _isCutting = false;
    [SerializeField] private int _dstThreshold = 50;
    private float _cutInterval = 0.25f, _cutTimer = 0.25f;

    private void Start()
    {
        _board = CuttingBoard.Instance;

        ingredientDurability = 100;
        // for debugging purposes
    }

    private void Update()
    {
        if (!_board.CanCut) return;

        if (Input.GetMouseButtonDown(0))
        {
            _cursorPoints.Clear();
            _isCutting = true;
            _cutTimer = 0;

            _cursorTrail.enabled = false;
            _cursorTrail.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(_cursorPos.x, _cursorPos.y, transform.position.z));
            _cursorTrail.Clear();
            _cursorTrail.enabled = true;
        }

        if (Input.GetMouseButton(0))
        {
            _cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            _cursorTrail.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(_cursorPos.x, _cursorPos.y, transform.position.z));
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isCutting = false;
            _cutTimer = _cutInterval;
            CompareCuts();  
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CompleteChopping();
        }
        // for debugging purposes, provides the player with a way to quit cutting
        // at least until i implement something else lmao

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
        int count = _cursorPoints.Count;

        if (count < 2) return;

        foreach (Transform t in _cutPoints)
        {
            float xPosition = Camera.main.WorldToScreenPoint(t.position).x;

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
        if (result) { _successfulCutCount++; }

        _cutCount++;
        ingredientDurability -= 10;
        ingredientDurability = Mathf.Clamp(ingredientDurability, 0f, 100f);

        CheckIngredientStatus();
    }
    void CompleteChopping()
    {
        Debug.Log("All portions are chopped. Yay!" + '\n' + RateChopping());

        GameObject ob = Instantiate(CuttingBoard.Instance.list.GetChoppedPrefab(gameObject.name.ToLower() + "-cut"));
        ob.transform.parent = transform.parent;
        // later, this will just grab the name of the scriptable object attached to the prefab.

        Destroy(gameObject);
    }
    string RateChopping()
    {
        if (_cutCount > _cursorPoints.Count()) { return "you cut too much!"; }
        else if(_cutCount == _cursorPoints.Count()) { return "you cut it perfectly!"; }
        else { return "you cut it too little lol"; }
    }
    void CheckIngredientStatus()
    {
        // would communicate with the ingredient's state but for now its just checking the number of cuts
        
    }
}
