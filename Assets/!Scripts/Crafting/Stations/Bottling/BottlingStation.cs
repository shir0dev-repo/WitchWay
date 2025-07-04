using UnityEngine;

public class BottlingStation : Singleton<BottlingStation>
{
    public Vector3 BottlePivotPosition => _bottlePivot.position;
    public Transform BottlePivot => _bottlePivot;
    [SerializeField] private Transform _bottlePivot;
    [SerializeField] private GameObject _pipe;

    public Bottle CurrentBottle => _bottle;
    private Bottle _bottle;

    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Bottle bottle)) return;

        if (_bottle != null) return;
        _bottle = bottle;

    }

    private void OnTriggerExit(Collider other)
    {
        if (_bottle == null) return;

        _bottle = null;
    }
}
