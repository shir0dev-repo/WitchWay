using UnityEngine;
using System.Collections;

public class PuzzleArcaneCircle : PuzzleBase
{
    [Header("Arcane Circle Settings")]
    [SerializeField] private Transform[] _standPositions;
    [SerializeField] private float _standDuration = 5f;
    [SerializeField] private float proximityDistance = 2f;
    [SerializeField] private float standingDistance = 1f;

    [Header("Visual References")]
    [SerializeField] private GameObject[] spotGlowEffects;

    private int activeSpotIndex = -1;
    private bool isPlayerStandingOnSpot = false;
    private float _timeSpentStanding = 0f;
    private Coroutine whisperCoroutine;
    private Transform playerTransform;


    protected override void Awake()
    {
        base.Awake();

        ActivateRandomSpot();
    }
    private void Start()
    {
        if (playerTransform == null)
            playerTransform = WZPlayerManager.Instance?.transform;
    }

    public override bool IsSolved()
    {
        CheckPlayerProximityToActiveSpot();
        CheckPlayerStandingOnActiveSpot();

        if (isPlayerStandingOnSpot && _timeSpentStanding >= _standDuration)
        {
            Debug.Log("Puzzle solved!");
            return true;
        }
        return false;
    }

    private void ActivateRandomSpot()
    {
        for (int i = 0; i < spotGlowEffects.Length; i++)
        {
            if (spotGlowEffects[i] != null)
                spotGlowEffects[i].SetActive(false);
        }

        activeSpotIndex = Random.Range(0, _standPositions.Length);
        if (spotGlowEffects[activeSpotIndex] != null)
            spotGlowEffects[activeSpotIndex].SetActive(true);

        Debug.Log($"Play subtle glow effect on spot {activeSpotIndex}");
    }

    private void CheckPlayerProximityToActiveSpot()
    {
        if (activeSpotIndex < 0) return;

        float distance = Vector3.Distance(playerTransform.position, _standPositions[activeSpotIndex].position);

        if (distance <= proximityDistance)
        {
            if (whisperCoroutine == null)
            {
                Debug.Log("Start playing whisper sounds");
                whisperCoroutine = StartCoroutine(PlayWhisperSounds());
            }
        }
        else
        {
            if (whisperCoroutine != null)
            {
                Debug.Log("Stop whisper sounds");
                StopCoroutine(whisperCoroutine);
                whisperCoroutine = null;
            }
        }
    }

    private void CheckPlayerStandingOnActiveSpot()
    {
        if (activeSpotIndex < 0) return;

        float distance = Vector3.Distance(playerTransform.position, _standPositions[activeSpotIndex].position);

        if (distance <= standingDistance)
        {
            if (!isPlayerStandingOnSpot)
            {
                isPlayerStandingOnSpot = true;
                _timeSpentStanding = 0f;
                Debug.Log("Player is now standing on the active spot");
            }

            _timeSpentStanding += Time.deltaTime;
        }
        else
        {
            if (isPlayerStandingOnSpot)
            {
                isPlayerStandingOnSpot = false;
                _timeSpentStanding = 0f;
                Debug.Log("Player is no longer standing on the active spot");
            }
        }
    }

    private IEnumerator PlayWhisperSounds()
    {
        while (true)
        {
            Debug.Log("Play whisper sound effect");
            yield return new WaitForSeconds(2f);
        }
    }

    protected override void OnSolvePuzzle()
    {
        if (whisperCoroutine != null)
        {
            StopCoroutine(whisperCoroutine);
            whisperCoroutine = null;
        }

        for (int i = 0; i < spotGlowEffects.Length; i++)
        {
            if (spotGlowEffects[i] != null)
                spotGlowEffects[i].SetActive(false);
        }
    }
}
