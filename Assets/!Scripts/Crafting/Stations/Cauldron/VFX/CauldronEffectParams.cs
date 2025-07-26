using UnityEngine;
using static Unity.Collections.AllocatorManager;

[CreateAssetMenu(fileName = "New Cauldron Effect Parameters", menuName = "Alchemy/Effect Parameters/Cauldron Effect Parameters")]
public class CauldronEffectParams : ScriptableObject
{
    [Header("Main")]
    public Texture2D MainTexture;
    [ColorUsage(showAlpha: false)] public Color Color = Color.white;
    [ColorUsage(showAlpha: false)] public Color FoamPrimary = Color.white;
    [Range(0, 1)] public float FoamStrengthPrimary = 1.0f;
    [ColorUsage(showAlpha: false)] public Color FoamSecondary = Color.white;
    [Range(0, 1)] public float FoamStrengthSecondary = 1.0f;
    public MaterialPropertyBlock ConvertToPropertyBlock()
    {
        return ConvertToPropertyBlock(this);
    }

    private static MaterialPropertyBlock ConvertToPropertyBlock(CauldronEffectParams e)
    {
        MaterialPropertyBlock block = new();

        block.SetColor("_Water_Color", e.Color);
        block.SetColor("_Primary_Foam_Color", e.FoamPrimary);
        block.SetColor("_Secondary_Foam_Color", e.FoamSecondary);
        block.SetFloat("_Primary_Foam_Strength", e.FoamStrengthPrimary);
        block.SetFloat("_Secondary_Foam_Strength", e.FoamStrengthSecondary);
        return block;
    }
}
