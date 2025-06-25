using UnityEngine;

public class CauldronMaster : MonoBehaviour
{
    public static CauldronMaster Instance { get; private set; }
    public IngredientsInPot InsidePot { get; private set; }
    public CauldronEvents CauldronEvents { get; private set; }

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
    }
    private void Update()
    {
        if (StationManager.Instance.GetCurrentStation() == 3)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ToggleMixing(!CurrentlyMixing);
            }
        }
        // press the spacebar to start/stop mixing WHEN ITS ON THE CORRECT STATION 
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
}
