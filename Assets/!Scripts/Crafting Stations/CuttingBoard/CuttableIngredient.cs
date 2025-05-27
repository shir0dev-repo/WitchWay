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

    CuttingBoard _board = null;

    List<Vector3> _cursorPoints = new List<Vector3>();
    Vector3 _cursorPos = Vector3.zero;
    
    private bool _isCutting = false;
    [SerializeField] private int _dstThreshold = 50;
    private float _cutInterval = 0.25f, _cutTimer = 0.25f;

    private void Start()
    {
        _board = CuttingBoard.Instance;
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
            _cursorTrail.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(_cursorPos.x, _cursorPos.y, 10));
            _cursorTrail.Clear();
            _cursorTrail.enabled = true;
        }

        if (Input.GetMouseButton(0))
        {
            _cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            _cursorTrail.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(_cursorPos.x, _cursorPos.y, 10));
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
                RemoveCutPoint(t);
                break;
            }
        }

        if (success)
        {
            Debug.Log("Yay!");
            UpdateChoppingProgress();
            // on a successful cut, add to the count
        }
    }
    void RemoveCutPoint(Transform t)
    {
        t.gameObject.SetActive(false);
        // removes the cut point so the player cannot cut the same spot twice
    }
    void UpdateChoppingProgress()
    {
        _cutCount++; 

        if(_cutCount == _cutPoints.Count()) { CompleteChopping(); }
        // when all of the sections are cut, complete the minigame
        else { return; }
    }
    void CompleteChopping()
    {
        Debug.Log("All portions are chopped. Yay!");

        Instantiate(CuttingBoard.Instance.list.GetChoppedPrefab(gameObject.name.ToLower() + "-cut"));
        // later, this will just grab the name of the scriptable object attached to the prefab.

        Destroy(gameObject);
    }
}
