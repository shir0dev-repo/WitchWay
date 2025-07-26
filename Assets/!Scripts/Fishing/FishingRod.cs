using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FishingRod : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private InputAction castAction;
    [Space(5)]
    [SerializeField] private InputAction reelAction;
    [Space(5)]
    [SerializeField] private InputAction exitEventAction;

    [Header("Casting Settings")]
    [SerializeField] private float castArcHeight;
    [SerializeField] private float castDuration;

    [Header("Fishing Settings")]
    [Tooltip("X for min, Y for max")]
    [SerializeField] private Vector2 catchTimeMinMax;

    [Header("Reeling Settings")]
    [SerializeField] private float reelStartAmount = 20;
    [SerializeField] private float mashIncreaseAmount = 5;
    [SerializeField] private float struggleDecreaseAmount = 2;

    [Header("Refs")]
    [SerializeField] private GameObject fishingUi;
    [SerializeField] private Slider reelingProgressSlider;
    [SerializeField] private GameObject fishingArea;
    [SerializeField] private GameObject bobberPrefab;
    [SerializeField] private Material lineMaterial;

    public enum Result
    {
        Nothing = 0,
        Item = 1,
        Jumpscare = 2
    }

    [Header("Drop Table")]
    [SerializeField] private FishingDropTable _dropTable = new FishingDropTable();

    private enum State
    {
        Nothing,
        Aiming,
        Casting,
        Fishing,
        Reeling,
        Caught
    }

    private State fishingState = State.Nothing;

    private WZPlayerInteract playerInteract;
    private WZPlayerController playerController;
    private FishingAreaPositionIndicator indicator;

    private GameObject activeBobber;
    private LineRenderer activeLine;

    public void Interact()
    {
        GameEvents.WitchingZone.OnFishingRodInteractedWith?.Invoke();

        SetupInputActions();

        if (indicator == null) indicator = fishingArea.GetComponent<FishingAreaPositionIndicator>();

        // Fishing mechanic starts here
        if (playerController == null) playerController = FindFirstObjectByType<WZPlayerController>();
        if (playerInteract == null) playerInteract = FindFirstObjectByType<WZPlayerInteract>();
        playerInteract.DisableReticle();
        playerInteract.SetInInteraction(true);

        playerController.EnableDisableAction(false,
            new PlayerControllerActions[] { PlayerControllerActions.moveAction, PlayerControllerActions.jumpAction, PlayerControllerActions.crouchAction });

        playerInteract.EnableDisableAction(false,
            new PlayerInteractActions[] { PlayerInteractActions.interactAction, PlayerInteractActions.dragAction });

        //show aimer
        indicator?.StartFollowing();
        fishingState = State.Aiming;
    }

    private void SetupInputActions()
    {
        castAction.Enable();
        reelAction.Enable();
        exitEventAction.Enable();

        castAction.started += CastLine;
        reelAction.started += ReelIn;
        exitEventAction.started += OnExitInput;
    }

    private void CastLine(InputAction.CallbackContext context)
    {
        if (fishingState != State.Aiming) return;

        fishingState = State.Casting;

        indicator.StopFollowing();
        Vector3 targetPos = indicator.GetPosition();

        GameObject bobber = Instantiate(bobberPrefab, playerInteract.transform.position, Quaternion.identity);
        activeBobber = bobber;
        activeLine = bobber.AddComponent<LineRenderer>();
        SetupLineRenderer(activeLine);
        StartCoroutine(AnimateBobberToTarget(bobber.transform, targetPos, castDuration));
    }

    public void SetupLineRenderer(LineRenderer lr)
    {
        lr.material = lineMaterial;
        lr.positionCount = 2;
        lr.startWidth = 0.02f;
        lr.endWidth = 0.015f;
        lr.numCapVertices = 4;
        lr.numCornerVertices = 2;

        lr.useWorldSpace = true;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
    }

    void Update()
    {
        //drawing rod line
        if ((fishingState == State.Casting) && activeBobber != null)
        {
            DrawFishingLine();
        }
        else if (fishingState == State.Reeling)
        {
            DrawFishingLineTense();

            if (reelingProgressSlider.value > reelingProgressSlider.minValue)
            {
                reelingProgressSlider.value -= struggleDecreaseAmount * Time.deltaTime;
                reelingProgressSlider.value = Mathf.Max(reelingProgressSlider.value, reelingProgressSlider.minValue);
            }
            else if (reelingProgressSlider.value <= reelingProgressSlider.minValue)
            {
                HandleResultNothing();
                fishingState = State.Caught;
            }
        }
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
        print("nothing");
        ExitFishing();
    }

    private void HandleResultItem(IngredientSO fishedIngredient)
    {
        // add item to inventory
        //SaveManager.Instance.CollectIngredient(fishedIngredient.name);

        Inventory inventory = GetComponent<Inventory>();
        inventory.AddNewItem(fishedIngredient);

        ExitFishing();
    }

    private void HandleResultJumpscare()
    {
        // gets implemented later
        GameEvents.WitchingZone.OnJumpscareRequested?.Invoke();

        print("jumpscare");
        ExitFishing();
    }

    private void DrawFishingLine()
    {
        if (activeLine == null) return;

        activeLine.positionCount = 3;
        Vector3 start = playerInteract.transform.position;
        Vector3 end = new Vector3(activeBobber.transform.position.x, activeBobber.transform.position.y + (activeBobber.transform.localScale.y / 2), activeBobber.transform.position.z);
        Vector3 middle = (start + end) * 0.5f;
        middle.y -= 0.5f;

        activeLine.SetPosition(0, start);
        activeLine.SetPosition(1, middle);
        activeLine.SetPosition(2, end);
    }

    private void DrawFishingLineTense()
    {
        if (activeLine == null) return;

        activeLine.positionCount = 2;
        Vector3 start = playerInteract.transform.position;
        Vector3 end = new Vector3(activeBobber.transform.position.x, activeBobber.transform.position.y + (activeBobber.transform.localScale.y / 2), activeBobber.transform.position.z);

        activeLine.SetPosition(0, start);
        activeLine.SetPosition(1, end);
    }

    private IEnumerator AnimateBobberToTarget(Transform bobber, Vector3 targetPos, float duration)
    {
        Vector3 startPos = bobber.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float time = elapsedTime / duration;

            Vector3 flatPos = Vector3.Lerp(startPos, targetPos, time);

            float height = 4 * castArcHeight * time * (1 - time);
            flatPos.y += height;

            bobber.position = flatPos;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bobber.position = targetPos;

        fishingState = State.Fishing;
        StartCoroutine(Fishing());
    }

    private IEnumerator Fishing()
    {
        yield return new WaitForSeconds(Random.Range(catchTimeMinMax.x, catchTimeMinMax.y));

        fishingState = State.Reeling;
        fishingUi.SetActive(true);
        reelingProgressSlider.value = reelStartAmount;
    }

    private void ReelIn(InputAction.CallbackContext context)
    {
        if (fishingState != State.Reeling) return;

        reelingProgressSlider.value += mashIncreaseAmount;

        if (reelingProgressSlider.value >= reelingProgressSlider.maxValue)
        {
            OnFishingSuccessful();
            fishingState = State.Caught;
        }
    }

    private void OnExitInput(InputAction.CallbackContext context) //wrapper for exit key
    {
        ExitFishing();
    }

    private void ExitFishing()
    {
        fishingState = State.Nothing;

        //diable fishing input
        castAction.started -= CastLine;
        reelAction.started -= ReelIn;
        exitEventAction.started -= OnExitInput;

        castAction.Disable();
        reelAction.Disable();
        exitEventAction.Disable();

        //reenable player
        if (playerController != null)
        {
            playerController.EnableDisableAction(true,
                new PlayerControllerActions[] { PlayerControllerActions.moveAction, PlayerControllerActions.jumpAction, PlayerControllerActions.crouchAction });
        }

        if (playerInteract != null)
        {
            playerInteract.EnableDisableAction(true,
                new PlayerInteractActions[] { PlayerInteractActions.interactAction, PlayerInteractActions.dragAction });

            playerInteract.EnableReticle();
            playerInteract.SetInInteraction(false);
        }

        //reset ui
        fishingUi.SetActive(false);
        reelingProgressSlider.value = 0;

        //stop indicator
        indicator.StopFollowing();

        //destory bobber and line
        if (activeLine != null) Destroy(activeLine);
        if (activeBobber != null) Destroy(activeBobber);

        activeBobber = null;
        activeLine = null;
    }
}
