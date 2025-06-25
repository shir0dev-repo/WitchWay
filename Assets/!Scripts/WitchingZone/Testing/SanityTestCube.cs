using UnityEngine;

public class SanityTestCube : MonoBehaviour
{
    [Header("Sanity Change Value")]
    [SerializeField] private int sanityChangeValue;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WZPlayerSanity playerSanity = other.GetComponent<WZPlayerSanity>();
            if (playerSanity != null)
            {
                if(sanityChangeValue > 0)
                {
                    playerSanity.IncreaseSanity(sanityChangeValue);
                }
                else if (sanityChangeValue < 0)
                {
                    playerSanity.DecreaseSanity(sanityChangeValue);
                }
                else
                {
                    Debug.Log("Sanity change value is zero, no change applied.");
                }
            }
        }
    }
}
