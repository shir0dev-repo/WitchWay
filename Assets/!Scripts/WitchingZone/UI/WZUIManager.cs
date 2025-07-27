using UnityEngine;
using UnityEngine.UI;

public class WZUIManager : MonoBehaviour
{
    [Header("Sanity")]
    [SerializeField] private Image sanityBar;
    [SerializeField] private WZPlayerSanity playerSanity;

    private void Start()
    {
        GameEvents.WitchingZone.OnSanityChanged += UpdateSanityBar;
        
        GameEvents.WitchingZone.OnPlayerSpawned += () =>
        {
            if (playerSanity == null)
            {
                playerSanity = WZPlayerManager.Instance.GetComponent<WZPlayerSanity>();
            }
            UpdateSanityBar();
        };
    }

    private void UpdateSanityBar()
    {
        if (sanityBar != null)
        {
            float fillAmount = (playerSanity.GetSanity() + 10) / 20f;
            sanityBar.fillAmount = fillAmount;
        }
    }
}
