using UnityEngine;

public class CuttingBoardCutFX : MonoBehaviour
{
    Color _particleColor = Color.white;
    [SerializeField] private ParticleSystem _cutPS;

    private void OnEnable()
    {
        GameEvents.Crafting.OnCutItem += SpawnParticleBurst;
        GameEvents.Crafting.OnItemPlacedOnCuttingBoard += SetParticleParams;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnCutItem -= SpawnParticleBurst;
        GameEvents.Crafting.OnItemPlacedOnCuttingBoard -= SetParticleParams;
    }

    private void SetParticleParams(WorldIngredient ingredient)
    {
        MeshRenderer mr = ingredient.GetComponentInChildren<MeshRenderer>(true);
        if (mr == null) return;

        _particleColor = mr.sharedMaterial.GetColor("_FXColor");

        var ps = _cutPS.main;
        ps.startColor = new ParticleSystem.MinMaxGradient(_particleColor);
    }

    private void SpawnParticleBurst(WorldIngredient ingredient, Transform cutPoint)
    {
        _cutPS.transform.position = cutPoint.position;
        _cutPS.Play();
    }
}
