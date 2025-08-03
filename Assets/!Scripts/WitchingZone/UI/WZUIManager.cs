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
