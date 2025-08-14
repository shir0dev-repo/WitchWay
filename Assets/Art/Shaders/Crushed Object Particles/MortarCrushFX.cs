using System;
using UnityEngine;

public class MortarCrushFX : MonoBehaviour
{
    Color _particleColor = Color.white;
    [SerializeField] private ParticleSystem _crushPS;

    private void OnEnable()
    {
        GameEvents.Crafting.OnItemDurabilityChanged += SpawnParticleBurst;
        GameEvents.Crafting.OnItemPlacedInMortar += SetParticleParams;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnItemDurabilityChanged -= SpawnParticleBurst;
        GameEvents.Crafting.OnItemPlacedInMortar -= SetParticleParams;
    }

    private void SetParticleParams(WorldIngredient ingredient)
    {
        MeshRenderer mr = ingredient.GetComponentInChildren<MeshRenderer>();
        if (mr == null) return;

        _particleColor = mr.sharedMaterial.GetColor("_FXColor");
        
        var ps = _crushPS.main;
        ps.startColor = new ParticleSystem.MinMaxGradient(_particleColor);
    }

    private void SpawnParticleBurst(WorldIngredient ingredient, float arg2)
    {
        _crushPS.Play();
    }
}
