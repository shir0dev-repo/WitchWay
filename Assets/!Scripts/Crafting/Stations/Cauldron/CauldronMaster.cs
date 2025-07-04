using UnityEngine;

public class CauldronMaster : MonoBehaviour
{
    public static CauldronMaster Instance { get; private set; }
    public IngredientsInPot InsidePot { get; private set; }
    public CauldronEvents CauldronEvents { get; private set; }
    public CauldronMixingDuration Duration { get; private set; }

    public bool CurrentlyMixing { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        InsidePot = GetComponentInChildren<IngredientsInPot>();
        CauldronEvents = GetComponentInChildren<CauldronEvents>();
        Duration = GetComponentInChildren<CauldronMixingDuration>();
    }
    private void Start()
    {
        CauldronEvents.ActivateMixing += ZoomIntoCauldron;
        CauldronEvents.DeactivateMixing += CameraManager.Instance.ResetZoom;
        CauldronEvents.DeactivateMixing += CauldronMixingEnded;
    }
    public void ToggleMixing(bool toggle)
    {
        if (CurrentlyMixing == toggle) return;

        CurrentlyMixing = toggle;
        if (CurrentlyMixing)
        {
            CauldronEvents.ActivateMixing?.Invoke();
            Debug.Log("start mixing!");
        }
        else
        {
            CauldronEvents.DeactivateMixing?.Invoke();
            Debug.Log("deactivating mixing.");
        }
    }
    void ZoomIntoCauldron()
    {
        CameraManager.Instance.ZoomIn(40);
    }
    void CauldronMixingEnded()
    {
        CurrentlyMixing = false;
    }
}
