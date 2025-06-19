using UnityEngine;

public class WZPlayerSanity : MonoBehaviour
{
    [SerializeField][Range(-10, 10)] private int playerSanity = 10;

    private void Start()
    {
        GameEvents.WitchingZone.OnSanityChanged += CheckSanityLevel;
    }

    public void ChangeSanity(int sanityChange) //passing a negative will decrease
    {
        playerSanity += sanityChange;
        playerSanity = Mathf.Clamp(playerSanity, -10, 10);

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
