using UnityEngine;

public class FishingRod : MonoBehaviour
{
    [SerializeField] private GameObject fishingArea;

    public enum Result
    {
        Nothing = 0,
        Item = 1,
        Jumpscare = 2
    }

    [SerializeField] private FishingDropTable _dropTable = new FishingDropTable();

    public void Interact()
    {
        GameEvents.WitchingZone.OnFishingRodInteractedWith?.Invoke();

        // Fishing mechanic starts here
        WZPlayerInteract playerInteract = FindFirstObjectByType<WZPlayerInteract>();
        playerInteract.DisableReticle();
        playerInteract.SetInInteraction(true);

        //show aimer
        fishingArea.GetComponent<FishingAreaPositionIndicator>()?.StartFollowing();
    }

    // call when finished fishing
    private void OnFishingSuccessful()
    {
        Result result = _dropTable.GetDrop(out IngredientSO ing);

        switch (result)
        {
            case Result.Nothing:
                HandleResultNothing();
                return;
            case Result.Item:
                HandleResultItem(ing);
                return;
            case Result.Jumpscare:
                HandleResultJumpscare();
                return;
        }
    }

    private void HandleResultNothing()
    {
        // might be used for textbox
    }

    private void HandleResultItem(IngredientSO fishedIngredient)
    {
        // add item to inventory
    }

    private void HandleResultJumpscare()
    {
        // gets implemented later
        GameEvents.WitchingZone.OnJumpscareRequested?.Invoke();
    }
}
