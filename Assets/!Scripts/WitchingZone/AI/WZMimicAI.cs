using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.Mathematics;
public class WZMimicAI : MonoBehaviour
{
    private bool isActive = false, isActivating, isDeactivating = false, isGrowling = false, isTwitching = false, isAttacking = false, isBeingLookedAt = false;
    private Vector3 startPos;
    private quaternion startRot;
    private float distanceToPlayer, stareTimer = 0f;
    [Header("References")]
    public Transform playerTransform;
    
    [Header("Mimic Settings")]
    [SerializeField] private float activationDelay;
    [SerializeField] private float deactivationDelay;
    [SerializeField] private float activationDistance;
    [SerializeField] private float deactivationDistance;
    [SerializeField] private float staringThreshold;
    [SerializeField] private float growlDelay;
    [SerializeField] private float twitchDelay;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackDistance;

    [Header("Look Detection")]
    [SerializeField] private float lookDetectDistance = 20f;
    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        isActive = false;
        isGrowling = false;
        isTwitching = false;
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);
        if (isActive == false)
        {
            if (isGrowling == false)
            {
                StartCoroutine(MimicGrowl());
                isGrowling = true;
            }
            if (isTwitching == false)
            {
                StartCoroutine(MimicTwitch());
                isTwitching = true;
            }

            

            if (playerTransform.position != null)
            {
                if (distanceToPlayer <= activationDistance && !isActivating)
                {
                    StopAllCoroutines();
                    StartCoroutine(ActivateMimic());
                    isActivating = true;
                }
            }
            Camera cam = Camera.main;
            if (cam != null)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, lookDetectDistance))
                {
                    isBeingLookedAt = true;
                    
                }else
                {
                    isBeingLookedAt = false;
                }
            }
            
            if (isBeingLookedAt)
            {
                if (stareTimer == 0f)
                {
                    Debug.Log("Mimic is being looked at, starting stare timer.");
                }
                stareTimer += Time.deltaTime;
                if (stareTimer >= staringThreshold && !isActivating)
                {
                    Debug.Log("Mimic activation triggered by staring.");
                    StopAllCoroutines();
                    StartCoroutine(ActivateMimic());
                    isActivating = true;
                }
            }
            else
            {
                stareTimer = 0f;
            }
        }
        else
        {
            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
            RaycastHit hit;
            Vector3 moveDir = dirToPlayer;
            float avoidStrength = 2f;
            if (Physics.Raycast(transform.position, dirToPlayer, out hit, 1.0f))
            {
                moveDir += hit.normal * avoidStrength;
            }
            transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;

            Vector3 lookDir = playerTransform.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 10f * Time.deltaTime);

            if (distanceToPlayer <= attackDistance && !isAttacking)
            {
                StartCoroutine(MimicAttack());
            }

            if (distanceToPlayer >= deactivationDistance && !isDeactivating)
            {
                isDeactivating = true;
                StartCoroutine(DeactivateMimic());
            }
        }
    }

    public IEnumerator ActivateMimic()
    {
        Debug.Log("Mimic Activativation started");
        isGrowling = true;
        isTwitching = true;
        yield return new WaitForSeconds(activationDelay);
        isActive = true;
        isActivating = false;
        Debug.Log("Mimic Activated");
    }
    public IEnumerator DeactivateMimic()
    {
        Debug.Log("Mimic Deactivation started");
        yield return new WaitForSeconds(deactivationDelay);
        isActive = false;
        isGrowling = false;
        isTwitching = false;
        isDeactivating = false;
        StartCoroutine(ReturnToStart());
    }

    private IEnumerator ReturnToStart()
    {
        while (Vector3.Distance(transform.position, startPos) > 0.1f)
        {
            Vector3 dir = (startPos - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;
        transform.rotation = startRot;
    }
    public IEnumerator MimicGrowl()
    {
        yield return new WaitForSeconds(growlDelay);

        Debug.Log("Mimic Growl Sound/Animation plays");
        isGrowling = false;
    }
    public IEnumerator MimicTwitch()
    {
        yield return new WaitForSeconds(twitchDelay);

        Debug.Log("Mimic Twitching Sound/Animation plays");
        isTwitching = false;
    }
    public IEnumerator MimicAttack()
    {
        Debug.Log("Mimic Attack started");
        isAttacking = true;
        yield return new WaitForSeconds(attackDelay);
        if (distanceToPlayer > attackDistance)
        {
            Debug.Log("Mimic Attack cancelled due to distance");
            isAttacking = false;
            yield break;
        }
        Debug.Log("Mimic Attack Sound/Animation plays");
        WZPlayerManager.Instance?.DecreaseSanity(2);
        isAttacking = false;
    }
    public void SetBeingLookedAt(bool lookedAt)
    {
        isBeingLookedAt = lookedAt;
    }
}
