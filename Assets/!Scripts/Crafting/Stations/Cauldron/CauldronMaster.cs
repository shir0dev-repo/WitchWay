using UnityEngine;

public class CauldronMaster : MonoBehaviour
{
    public static CauldronMaster Instance { get; private set; }
    public IngredientsInPot InsidePot { get; private set; }
    public SwitchToMixing CauldronEvents { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        InsidePot = GetComponentInChildren<IngredientsInPot>();
        CauldronEvents = GetComponentInChildren<SwitchToMixing>();
    }
}
