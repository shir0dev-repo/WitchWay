using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class WZDialogueMonsterAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform monsterTransform;

    [Header("Disappearance Settings")]
    [SerializeField] private float disappearDistance = 10f;
    [SerializeField] private float disappearDelay = 5f;
    [SerializeField] private float groundCheckDistance = 2f;

    [Header("Spawning Settings")]
    [SerializeField] private float spawnDistance = 5f;
    [SerializeField] private float spawnHeight = 2f;
    [SerializeField] private float spawnCheckRadius = 0.5f;
    [SerializeField] private float spawnCheckDelay = 1f;
    [SerializeField] private float spawnChance = 0.5f;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private bool isMonsterVisible = true, isDialogueCompleted = false;
    private float disappearTimer = 0f;
    private Coroutine reappearRoutine;

    [HideInInspector] public bool hasStarted = false;

    void Update()
    {
        if (isDialogueCompleted || !hasStarted)
        {
            return;
        }

        if (isMonsterVisible && playerTransform != null)
        {
            Vector3 lookDir = playerTransform.position - monsterTransform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                monsterTransform.rotation = Quaternion.Slerp(monsterTransform.rotation, Quaternion.LookRotation(lookDir), 10f * Time.deltaTime);

            float dist = Vector3.Distance(playerTransform.position, monsterTransform.position);
            if (dist > disappearDistance)
            {
                disappearTimer += Time.deltaTime;
                if (debugMode)
                {
                    Debug.Log($"Player is too far away. Distance: {dist}, Timer: {disappearTimer}");
                }
                if (disappearTimer >= disappearDelay)
                {
                    DisappearMonster();
                    disappearTimer = 0f;
                }
            }
            else
            {
                disappearTimer = 0f;
            }
        }
    }

    public void DisappearMonster()
    {
        isMonsterVisible = false;
        monsterTransform.gameObject.SetActive(false);
    }
    public void CompleteDialogue()
    {
        isDialogueCompleted = true;
        if (debugMode)
        {
            Debug.Log("Dialogue completed. Monster will disappear.");
        }
        StartCoroutine(DisappearMonsterCoroutine());
    }

    private IEnumerator DisappearMonsterCoroutine()
    {
        yield return new WaitForSeconds(disappearDelay);

        DisappearMonster();
    }

    private IEnumerator ReappearBehindPlayerCoroutine()
    {
        while (true)
        {

            yield return new WaitForSeconds(spawnCheckDelay);

            Vector3 behindDir = -playerTransform.forward;
            Vector3 spawnPos = playerTransform.position + behindDir * spawnDistance;

            for (int i = 0; i < 10; i++)
            {
                Vector3 checkPos = spawnPos + Random.insideUnitSphere * 1.5f;
                checkPos.y = playerTransform.position.y;

                if (Physics.CheckSphere(checkPos, spawnCheckRadius, obstacleMask))
                {
                    if (debugMode)
                    {
                        Debug.Log($"Position {checkPos} is blocked by an obstacle. Retrying...");
                    }
                    continue;
                }

                Ray groundRay = new Ray(checkPos + Vector3.up, Vector3.down);
                if (Physics.Raycast(groundRay, out RaycastHit hit, groundCheckDistance, groundMask))
                {
                    if (debugMode)
                    {
                        Debug.Log($"Monster reappearing at: {hit.point}");
                    }
                    monsterTransform.position = hit.point + Vector3.up * spawnHeight;
                    monsterTransform.gameObject.SetActive(true);
                    isMonsterVisible = true;
                    reappearRoutine = null;
                    yield break;
                }
            }
        }
    }

    public void StartChase(Vector3 startPosition, Quaternion startRotation)
    {
        if (isDialogueCompleted)
            return;

        if (debugMode)
        {
            Debug.Log($"Starting chase at position: {startPosition}, rotation: {startRotation}");
        }
        monsterTransform.position = startPosition;
        monsterTransform.rotation = startRotation;
        hasStarted = true;

        if (reappearRoutine != null)
        {
            StopCoroutine(reappearRoutine);
            reappearRoutine = null;
        }
    }

    public void RoomEntered()
    {
        if (!hasStarted || isDialogueCompleted)
            return;

        if (debugMode)
            Debug.Log("Player entered a new room, monster is trying to spawn.");

        if (Random.value < spawnChance)
        {
            StartCoroutine(ReappearBehindPlayerCoroutine());
        }
    }
}
