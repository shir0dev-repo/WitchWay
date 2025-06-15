using UnityEngine;

public class WZPlayerSanity : MonoBehaviour
{
    [SerializeField][Range(-10, 10)] private int playerSanity = 10;

    public void ChangeSanity(int sanityChange) //passing a negative will decrease
    {
        playerSanity += sanityChange;
        playerSanity = Mathf.Clamp(playerSanity, -10, 10);

        CheckSanityLevel();
    }

    private void CheckSanityLevel()
    {
        if (playerSanity <= -10)
        {
            //run game over
        }
    }
}
