using UnityEngine;

public class WZPlayerManager : MonoBehaviour
{
    public static WZPlayerManager Instance;
    private WZPlayerController playerController;
    private WZPlayerSanity playerSanity;
    private WZPlayerInteract playerInteract;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        playerController = GetComponent<WZPlayerController>();
        playerSanity = GetComponent<WZPlayerSanity>();
    }

    public void SetCanMove(bool canMove)
    {
        if (playerController != null)
            playerController.SetCanMove(canMove);

        if (playerInteract != null)
            playerInteract.SetControlsEnabled(canMove);
    }

    public void IncreaseSanity(int amount)
    {
        if (playerSanity != null)
            playerSanity.ChangeSanity(Mathf.Abs(amount));
    }

    public void DecreaseSanity(int amount)
    {
        if (playerSanity != null)
            playerSanity.ChangeSanity(-Mathf.Abs(amount));
    }

    public void SetCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}