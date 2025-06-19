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
                playerSanity.ChangeSanity(sanityChangeValue);
            }
        }
    }
}
