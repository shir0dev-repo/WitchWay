using UnityEngine;
using System.Collections;

public class PuzzleArcaneCircle : PuzzleBase
{
    [Header("Arcane Circle Settings")]
    [SerializeField] private Transform[] _standPositions;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private Transform chestSpawnPoint;
    [SerializeField] private float _standDuration = 5f;
    [SerializeField] private float proximityDistance = 2f;
    [SerializeField] private float standingDistance = 1f;

    [Header("Visual References")]
    [SerializeField] private GameObject[] spotGlowEffects;

    private int activeSpotIndex = -1;
    private bool isPlayerStandingOnSpot = false;
    private float _timeSpentStanding = 0f;
    private Coroutine whisperCoroutine;
    private GameObject spawnedChest;

    protected override void Awake()
    {
        base.Awake();

        ActivateRandomSpot();
        
        if (playerTransform == null)
            playerTransform = WZPlayerManager.Instance?.transform;
    }

    void Update()
    {
        if (HasBeenSolved || playerTransform == null) return;

        CheckPlayerProximityToActiveSpot();
        CheckPlayerStandingOnActiveSpot();
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

        Debug.Log($"// TODO: Play subtle glow effect on spot {activeSpotIndex}");
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
                Debug.Log("Intensify whisper sounds");
            }

            _timeSpentStanding += Time.deltaTime;

            if (_timeSpentStanding >= _standDuration)
            {
                SpawnChest();
            }
        }
        else
        {
            if (isPlayerStandingOnSpot)
            {
                isPlayerStandingOnSpot = false;
                _timeSpentStanding = 0f;
                Debug.Log("Reset whisper intensity to normal");
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

    private void SpawnChest()
    {
        if (spawnedChest != null) return;

        Debug.Log("Play chest spawn sound effect");
        Debug.Log("Play chest spawn visual effect");

        if (whisperCoroutine != null)
        {
            StopCoroutine(whisperCoroutine);
            whisperCoroutine = null;
        }

        spawnedChest = Instantiate(chestPrefab, chestSpawnPoint.position, chestSpawnPoint.rotation);
        
        if (spotGlowEffects[activeSpotIndex] != null)
            spotGlowEffects[activeSpotIndex].SetActive(false);

        Debug.Log("Chest spawned! Puzzle solved.");
    }

    public override bool IsSolved()
    {
        return spawnedChest != null;
    }

    protected override void OnSolvePuzzle()
    {
        Debug.Log("Play puzzle completion sound");
        Debug.Log("Play puzzle completion visual effects");

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
