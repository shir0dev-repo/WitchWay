using UnityEngine;

public class WZDoor : MonoBehaviour
{

    private bool isOpen = false;

    private void Start()
    {
        GameEvents.WitchingZone.OnDoorUnlocked += OpenDoor;
    }

    public void Interact()
    {
        if (!isOpen) return;

        gameObject.SetActive(false);
    }

    public void OpenDoor()
    {
        isOpen = true;
    }
}
