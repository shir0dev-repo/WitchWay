using Shir0dev.LiquidFX;
using UnityEngine;

public class BottleVisuals : MonoBehaviour
{
    [SerializeField] private GameObject _cork;
    [SerializeField] private LiquidFX _potionFX;
    [SerializeField] private Transform _bottleHolderPivot;

    public Transform GetPivotForHolder() => _bottleHolderPivot;

    private void OnEnable()
    {
        GameEvents.Crafting.OnBottlePlacedInBottler += SetBottlePosition;
        GameEvents.Crafting.OnBottleFilled += FinishPotion;
        GameEvents.Crafting.OnBottleFillChanged += _potionFX.SetFillAmount;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnBottlePlacedInBottler -= SetBottlePosition;
        GameEvents.Crafting.OnBottleFilled -= FinishPotion;
        GameEvents.Crafting.OnBottleFillChanged -= _potionFX.SetFillAmount;
    }

    private void Start()
    {
        _cork.SetActive(false);
        _potionFX.SetFillAmount(0);
    }

    private void SetBottlePosition(Vector3 bottlerPivot)
    {
        transform.position = bottlerPivot - _bottleHolderPivot.localPosition;
    }

    private void FinishPotion(PotionData data)
    {
        _cork.SetActive(true);
    }
}
