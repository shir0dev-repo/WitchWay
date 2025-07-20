using UnityEngine;

public class WZPlayerManager : Singleton<WZPlayerManager>
{
    private WZPlayerController playerController;
    private WZPlayerSanity playerSanity;
    private WZPlayerInteract playerInteract;

    protected override void Awake()
    {
        base.Awake();

        playerController = GetComponent<WZPlayerController>();
        playerSanity = GetComponent<WZPlayerSanity>();
    }

    public void ToggleInput(bool toggle)
    {
        if (playerController != null)
            playerController.SetCanMove(toggle);

        if (playerInteract != null)
            playerInteract.SetControlsEnabled(toggle);
    }

    public void IncreaseSanity(int amount)
    {
        if (playerSanity != null)
            playerSanity.IncreaseSanity(Mathf.Abs(amount));
    }

    public void DecreaseSanity(int amount)
    {
        if (playerSanity != null)
            playerSanity.DecreaseSanity(Mathf.Abs(amount));
    }

    public void ToggleCursor(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}