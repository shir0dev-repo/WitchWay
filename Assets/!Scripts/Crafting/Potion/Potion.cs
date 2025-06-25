using Shir0dev.LiquidFX;
using UnityEngine;

[System.Serializable]
public class Potion : MonoBehaviour
{
    [SerializeField] private PotionData _data;

    [Header("VFX")]
    [SerializeField] private PotionEffectParams VFXParams;
    [SerializeField] private LiquidFX _liquidVFX;
}
