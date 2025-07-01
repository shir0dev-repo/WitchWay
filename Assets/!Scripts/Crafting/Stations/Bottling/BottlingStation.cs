using UnityEngine;

public class BottlingStation : Singleton<BottlingStation>
{
    public Vector3 BottlePivotPosition => _bottlePivot.position;
    [SerializeField] private Transform _bottlePivot;


    private void Start()
    {
        GameEvents.Crafting.OnBottlePlacedInBottler?.Invoke(BottlePivotPosition);
    }

}
