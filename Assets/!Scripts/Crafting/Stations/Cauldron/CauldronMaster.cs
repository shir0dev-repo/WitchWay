using UnityEngine;

public class CauldronMaster : MonoBehaviour
{
    public static CauldronMaster Instance { get; private set; }
    public IngredientsInPot InsidePot { get; private set; }
    public CauldronEvents CauldronEvents { get; private set; }

    bool isCurrentlyMixing = false;
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
            if (Input.GetKeyDown(KeyCode.Space) && !isCurrentlyMixing)
            {
                CauldronEvents.ActivateMixing?.Invoke();
                isCurrentlyMixing = true;
                Debug.Log("start mixing!");
            }
            else if (Input.GetKeyDown(KeyCode.Space) && isCurrentlyMixing)
            {
                CauldronEvents.DeactivateMixing?.Invoke();
                GameEvents.Crafting.OnCauldronMixStepCompleted?.Invoke();
                isCurrentlyMixing = false;
                Debug.Log("deactivating mixing.");
            }
        } 
        // press the spacebar to start/stop mixing WHEN ITS ON THE CORRECT STATION 
        
    }
}
