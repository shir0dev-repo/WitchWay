using UnityEngine;

public class WZPlayerSanity : MonoBehaviour
{
    private const int _SANITY_LIMIT = 10;
    [SerializeField, Range(-10, 10)] private int playerSanity = 10;

    private void Start()
    {
        GameEvents.WitchingZone.OnSanityChanged += CheckSanityLevel;
    }

    public void ModifySanity(int sanityChange)
    {
        if (sanityChange == 0) return;

        int prevSanity = playerSanity;
        playerSanity = Mathf.Clamp(playerSanity + sanityChange, -_SANITY_LIMIT, _SANITY_LIMIT);

        if (prevSanity > playerSanity)
            GameEvents.WitchingZone.OnSanityDecreased?.Invoke();
        else 
            GameEvents.WitchingZone.OnSanityIncreased?.Invoke();

        GameEvents.WitchingZone.OnSanityChanged?.Invoke();
    }

    private void CheckSanityLevel()
    {
        if (playerSanity <= -10)
        {
            //run game over
        }
    }

    public int GetSanity()
    {
        return playerSanity;
    }
}
