using UnityEngine;

[CreateAssetMenu(fileName = "New Potion Effect Parameters", menuName = "Alchemy/Effect Parameters/Potion Effect Parameters")]
public class PotionEffectParams : ScriptableObject
{
    public enum PropertyBlockType { Liquid = 0, FlowingWater = 1 }

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

    public void CopyTo(PotionEffectParams target)
    {
        CopyTo(this, target);
    }

    public MaterialPropertyBlock ConvertToPropertyBlock(PropertyBlockType type)
    {
        MaterialPropertyBlock block = new();

        return type switch
        {
            PropertyBlockType.Liquid => ConvertToLiquidPropertyBlock(this, block),
            PropertyBlockType.FlowingWater => ConvertToFlowingWaterPropertyBlock(this, block),
            _ => throw new System.ArgumentException("Invalid type specified!")
        };
    }

    public void ConvertToPropertyBlock(PropertyBlockType type, MaterialPropertyBlock block)
    {
        _ = type switch
        {
            PropertyBlockType.Liquid => ConvertToLiquidPropertyBlock(this, block),
            PropertyBlockType.FlowingWater => ConvertToFlowingWaterPropertyBlock(this, block),
            _ => throw new System.ArgumentException("Invalid type specified!")
        };
    }

    private static void CopyTo(PotionEffectParams from, PotionEffectParams to)
    {
        to.MainTexture = from.MainTexture;
        to.Tint = from.Tint;

        to.FoamColor = from.FoamColor;
        to.FoamLineWidth = from.FoamLineWidth;
        to.FoamLineSmoothing = from.FoamLineSmoothing;

        to.RimColor = from.RimColor;
        to.RimPower = from.RimPower;
    }

    private static MaterialPropertyBlock ConvertToLiquidPropertyBlock(PotionEffectParams e, MaterialPropertyBlock block)
    {
        return null;
    }

    private static MaterialPropertyBlock ConvertToFlowingWaterPropertyBlock(PotionEffectParams e, MaterialPropertyBlock block)
    {
        block.SetColor("_Water_Color", e.Tint);
        block.SetColor("_Primary_Foam_Color", e.FoamColor);
        block.SetColor("_Secondary_Foam_Color", e.RimColor);

        return block;
    }
}
