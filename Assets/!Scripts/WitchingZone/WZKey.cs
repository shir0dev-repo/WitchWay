using UnityEngine;

public class WZKey : MonoBehaviour
{
    [SerializeField] private WZDoor door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameEvents.WitchingZone.OnDoorUnlocked?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
