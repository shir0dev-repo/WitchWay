using UnityEngine;

//forcre recomiple
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

    public void ModifySanity(int change)
    {
        if (playerSanity != null)
            playerSanity.ModifySanity(change);
    }

    public void ToggleCursor(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}