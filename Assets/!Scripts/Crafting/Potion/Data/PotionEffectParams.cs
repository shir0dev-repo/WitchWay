using UnityEngine;

[CreateAssetMenu(fileName = "New Potion Effect Parameters", menuName = "Alchemy/Potions/New Potion Effect Parameters")]
public class PotionEffectParams : ScriptableObject
{
    [Header("Main")]
    public Texture2D MainTexture;
    [ColorUsage(showAlpha: false, hdr: true)] public Color Tint = Color.white;

    [Header("Foam")]
    [ColorUsage(showAlpha: false, hdr: true)] public Color FoamColor = Color.white;
    [Range(0, 1)] public float FoamLineWidth = 0;
    [Range(0, 0.1f)] public float FoamLineSmoothing = 0;

    [Header("Rim")]
    [ColorUsage(showAlpha: false, hdr: true)] public Color RimColor = Color.white;
    [Range(0, 10.0f)] public float RimPower = 10.0f;
}
