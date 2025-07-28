using System;
using UnityEngine;

public class BottlingStation : Singleton<BottlingStation>
{
    public Vector3 BottlePivotPosition => _bottlePivot.position;
    public Transform BottlePivot => _bottlePivot;
    [Header("References")]
    [SerializeField] private Transform _bottlePivot;
    [SerializeField] private GameObject _pipe;
    
    [Space]
    [SerializeField] private Siphon _siphon;

    public Bottle CurrentBottle => _bottle;
    private Bottle _bottle;

    private void OnEnable()
    {
        GameEvents.Crafting.OnObjectRemovedFromCursor += AttachBottle;
        GameEvents.Crafting.OnBottleFilled += FinishBottling;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnObjectRemovedFromCursor -= AttachBottle;
    }

    private void AttachBottle(IFollowCursor cursor)
    {
        if (cursor is not Bottle bottle)
        {
            _pipe.SetActive(false);
            return;
        }

        if (_bottle != null && bottle == _bottle)
        {
            _bottle.transform.position = BottlePivot.position;
            GameEvents.Crafting.OnBottlePlacedInBottler?.Invoke(_bottle);
            ToggleRelevantComponents(true);
        }
        else
        {
            GameEvents.Crafting.OnBottleRemovedFromBottler?.Invoke(_bottle);
            ToggleRelevantComponents(false);
        }
    }

    private void FinishBottling()
    {
        _bottle = null;
        _pipe.SetActive(false);
    }

    private void ToggleRelevantComponents(bool toggle)
    {
        _pipe.SetActive(toggle);
        _siphon.enabled = toggle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Bottle bottle)) return;

        if (bottle.CanBeBottled == false || _bottle != null)
            return;
        else
            _bottle = bottle;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_bottle == null) return;
        _bottle = null;
    }
}
