using System;
using System.Collections.Generic;
using UnityEngine;

public class KnifeTool : ToolBase
{
    [Header("Pivot")]
    [SerializeField] Vector3 cutRotationEulers;
    [SerializeField] Vector3 restRotationEulers;
    
    [Space]
    [SerializeField] private int _dstThreshold = 50;
    
    List<Vector3> _cursorPoints = new List<Vector3>();
    Vector3 _cursorPos = Vector3.zero;
    CuttingBoard _board;

    private bool _isCutting = false;
    private float _cutInterval = 0.25f;
    private float _cutTimer = 0.25f;

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
            _cutTimer = 0;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _cutTimer = _cutInterval;
            _isCutting = false;
            CheckCut();
            return;
        }

        _isCutting = Input.GetMouseButton(0);

        if (!_isCutting) return;
        
        _cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);

        if (_cutTimer > 0)
            _cutTimer -= Time.deltaTime;

        if (_cutTimer <= 0)
        {
            if (_cursorPoints.Count == 0 || Vector3.Distance(_cursorPos, _cursorPoints[^1]) > _dstThreshold)
            {
                _cutTimer += _cutInterval;
                _cursorPoints.Add(_cursorPos);
            }
        }
    }

    private void CheckCut()
    {
        if (_board == null) return;
        else if (!_board.HasIngredient) return;
        
        CuttableIngredient cuttable = _board.CurrentIngredient;
        
        if (cuttable.CompareCuts(_cursorPoints, out Transform cutPoint))
        {
            cuttable.TryDetachSegment(cutPoint);
            GameEvents.Crafting.OnCutItem?.Invoke(cuttable.Ingredient, cutPoint);

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayOneShot(_board.onKnifeCutSound, cutPoint.position);
        }
    }

    protected override void OnToolSelected()
    {
        gameObject.transform.rotation = Quaternion.Euler(cutRotationEulers);
        SoundManager.Instance.PlayOneShot(onToolSelected, this.transform.position);
    }
    
    protected override void OnToolDeselected()
    {
        gameObject.transform.rotation = Quaternion.Euler(restRotationEulers);
        SoundManager.Instance.PlayOneShot(onToolDeselected, this.transform.position);
    }
}
