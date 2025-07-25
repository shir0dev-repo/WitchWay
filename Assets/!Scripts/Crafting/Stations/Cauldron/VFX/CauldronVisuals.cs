using System;
using System.Collections.Generic;
using UnityEngine;

public class CauldronVisuals : MonoBehaviour
{
    private const string _WATER = "_Water_Color";
    private const string _FOAM_PRIMARY = "_Primary_Foam_Color";
    private const string _FOAM_SECONDARY = "_Secondary_Foam_Color";

    [Header("References")]
    [SerializeField] private MeshRenderer _renderer = null;

    private MaterialPropertyBlock _initialBlock = null;
    private MaterialPropertyBlock _targetBlock = null;

    private void OnEnable()
    {
        GameEvents.Crafting.OnCauldronMixProgressIncreased += UpdateRenderer;
        GameEvents.Crafting.OnCauldronMixSequenceCompleted += ClearTargetPropertyBlock;
    }

    private void OnDisable()
    {   
        GameEvents.Crafting.OnCauldronMixProgressIncreased -= UpdateRenderer;
        GameEvents.Crafting.OnCauldronMixSequenceCompleted -= ClearTargetPropertyBlock;
    }

    private void Start()
    {
        InitializeRenderer();
    }

    private void InitializeRenderer()
    {
        if (_renderer == null || !TryGetComponent(out _renderer))
        {
            Debug.LogWarning("No renderer found on CauldronVisuals!");
            return;
        }

        Material m = _renderer.material;

        _initialBlock = new MaterialPropertyBlock();
        _initialBlock.SetColor(_WATER, m.GetColor(_WATER));
        _initialBlock.SetColor(_FOAM_PRIMARY, m.GetColor(_FOAM_PRIMARY));
        _initialBlock.SetColor(_FOAM_SECONDARY, m.GetColor(_FOAM_SECONDARY));
    }

    private void ClearTargetPropertyBlock()
    {
        _targetBlock = null;
    }

    public void SetTargetPropertyBlock(CauldronEffectParams effectParams)
    {
        if (effectParams != null)
            _targetBlock = effectParams.ConvertToPropertyBlock();
    }

    private void UpdateRenderer(float mixProgress)
    {
        if (_targetBlock == null) return;
        mixProgress = Mathf.Clamp01(mixProgress);
        Debug.Log(mixProgress);

        MaterialPropertyBlock lerp = InterpolateMaterialProperties(_initialBlock, _targetBlock, mixProgress);

        _renderer.material.SetColor(_WATER, lerp.GetColor(_WATER));
        _renderer.material.SetColor(_FOAM_PRIMARY, lerp.GetColor(_FOAM_PRIMARY));
        _renderer.material.SetColor(_FOAM_SECONDARY, lerp.GetColor(_FOAM_SECONDARY));
    }

    private MaterialPropertyBlock InterpolateMaterialProperties(MaterialPropertyBlock a, MaterialPropertyBlock b, float t)
    {
        MaterialPropertyBlock result = new();
        
        Color waterColor = Color.Lerp(a.GetColor(_WATER), b.GetColor(_WATER), t);
        Color foamColorPrimary = Color.Lerp(a.GetColor(_FOAM_PRIMARY), b.GetColor(_FOAM_PRIMARY), t);
        Color foamColorSecondary = Color.Lerp(a.GetColor(_FOAM_SECONDARY), b.GetColor(_FOAM_SECONDARY), t);

        result.SetColor(_WATER, waterColor);
        result.SetColor(_FOAM_PRIMARY, foamColorPrimary);
        result.SetColor(_FOAM_SECONDARY, foamColorSecondary);

        return result;
    }
}
