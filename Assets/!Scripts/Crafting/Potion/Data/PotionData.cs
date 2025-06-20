using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Potion Data", menuName = "Alchemy/Potions/New Potion Data")]
public class PotionData : ScriptableObject
{
    public PotionEffect Effect;

    [Header("Shop")]
    public int Value;

    [Header("VFX")]
    public PotionEffectParams EffectParams;
}
