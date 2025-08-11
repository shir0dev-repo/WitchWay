using UnityEngine;
using System.Collections;

public class WZMimicAI : MonoBehaviour
{
    private static Vector2 ScreenCenter => new(Screen.width * 0.5f, Screen.height * 0.5f);

    [SerializeField] private WZMimicData _state;

    [Header("References")]
    [SerializeField] Transform playerTransform;

    [Header("Activation Settings")]
    [SerializeField] private float activationDelay;
    [SerializeField] private float deactivationDelay;
    [SerializeField] private float activationDistance;
    [SerializeField] private float deactivationDistance;
    [SerializeField] private float stareTimerThreshold;
    [SerializeField] private float growlDelay;
    [SerializeField] private float twitchDelay;

    [Header("Locomotion Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackDistance;
    
    [Header("Jump Arc Settings")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float jumpDistance;
    [SerializeField] private float jumpDelay;

    [Header("Look Detection")]
    [SerializeField] private float lookDetectDistance = 20f;

    private float distanceToPlayer;
    private float stareTimer = 0f;
    private bool isAirborne = false;

    void Start()
    {
        _state = new WZMimicData(transform.position, transform.rotation);
        if (playerTransform == null)
        {
            playerTransform = WZPlayerManager.Instance.transform;
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);
        }
        else
        {
            playerTransform = WZPlayerManager.Instance.transform;
        }

        if (_state.IsActive == false)
        {
            HandleInactiveState();
        }
        else
        {
            HandleActiveState();
        }
    }

    private void HandleInactiveState()
    {
        if (_state.IsGrowling == false)
        {
            _state.IsGrowling = true;
            StartCoroutine(MimicGrowl());
        }
        if (_state.IsTwitching == false)
        {
            _state.IsTwitching = true;
            StartCoroutine(MimicTwitch());
        }

        if (playerTransform.position != null)
        {
            if (distanceToPlayer <= activationDistance && !_state.IsActivating)
            {
                _state.IsActivating = true;
                StopAllCoroutines();
                StartCoroutine(ActivateMimic());
            }
        }

        Camera cam = Camera.main;
        if (cam != null)
        {
            // Don't bother with Input.mousePosition, as the cursor should always be locked to center of screen
            Ray ray = cam.ScreenPointToRay(ScreenCenter);
            if (Physics.Raycast(ray, out RaycastHit hit, lookDetectDistance) && hit.transform == transform)
            {
                _state.IsBeingLookedAt = true;
            }
            else
            {
                _state.IsBeingLookedAt = false;
            }
        }

        if (_state.IsBeingLookedAt)
        {
            if (stareTimer == 0f)
            {
                Debug.Log("Mimic is being looked at, starting stare timer.");
            }

            stareTimer += Time.deltaTime;
            if (stareTimer >= stareTimerThreshold && !_state.IsActivating)
            {
                _state.IsActivating = true;

                Debug.Log("Mimic activation triggered by staring.");
                StopAllCoroutines();
                StartCoroutine(ActivateMimic());
            }
        }
        else
        {
            stareTimer = 0f;
        }
    }

    private void HandleActiveState()
    {
        if (!_state.IsJumping)
        {
            _state.IsJumping = true;
            StartCoroutine(SineJumpMovement(jumpHeight, jumpDuration));
        }

        /*Vector3 dirToPlayer = (playerTransform.position - transform.position);
        dirToPlayer.y = 0;
        dirToPlayer.Normalize();
        Vector3 moveDir = dirToPlayer;
        float avoidStrength = 2f;
        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, 1.0f))
        {
            moveDir += hit.normal * avoidStrength;
        }
        transform.position += moveSpeed * Time.deltaTime * moveDir.normalized;
        */
        
        Vector3 lookDir = playerTransform.position - transform.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), turnSpeed * Time.deltaTime);

        if (distanceToPlayer <= attackDistance && !_state.IsAttacking)
        {
            StartCoroutine(MimicAttack());
        }

        if (distanceToPlayer >= deactivationDistance && !_state.IsDeactivating)
        {
            _state.IsDeactivating = true;
            StartCoroutine(DeactivateMimic());
        }
    }

    private IEnumerator ActivateMimic()
    {
        Debug.Log("Mimic Activativation started");
        _state.IsGrowling = true;
        _state.IsTwitching = true;
        yield return new WaitForSeconds(activationDelay);
        _state.IsActive = true;
        _state.IsActivating = false;
        Debug.Log("Mimic Activated");
    }
    private IEnumerator DeactivateMimic()
    {
        Debug.Log("Mimic Deactivation started");
        yield return new WaitForSeconds(deactivationDelay);
        _state.IsActive = false;
        _state.IsGrowling = false;
        _state.IsTwitching = false;
        _state.IsDeactivating = false;
        StartCoroutine(ReturnToStart());
    }

    private IEnumerator ReturnToStart()
    {
        Vector3 startPos = _state.StartPosition;
        
        while (Vector3.Distance(transform.position, startPos) > 0.1f)
        {
            Vector3 dir = (startPos - transform.position).normalized;
            transform.position += moveSpeed * Time.deltaTime * dir;
            yield return null;
        }

        transform.position = startPos;
        
        transform.rotation = _state.StartRotation;
    }
    public IEnumerator MimicGrowl()
    {
        Debug.Log("Mimic Growl Sound/Animation plays");

        yield return new WaitForSeconds(growlDelay);
        _state.IsGrowling = false;
    }
    public IEnumerator MimicTwitch()
    {
        Debug.Log("Mimic Twitching Sound/Animation plays");

        yield return new WaitForSeconds(twitchDelay);
        _state.IsTwitching = false;
    }
    public IEnumerator MimicAttack()
    {
        Debug.Log("Mimic Attack started");
        _state.IsAttacking = true;
        yield return new WaitForSeconds(attackDelay);
        if (distanceToPlayer > attackDistance)
        {
            Debug.Log("Mimic Attack cancelled due to distance");
            _state.IsAttacking = false;
            yield break;
        }
        Debug.Log("Mimic Attack Sound/Animation plays");
        if (WZPlayerManager.Instance != null)
            WZPlayerManager.Instance.ModifySanity(-2);

        _state.IsAttacking = false;
    }

    public void SetBeingLookedAt(bool lookedAt)
    {
        _state.IsBeingLookedAt = lookedAt;
    }

    private Vector3 GetLandingPosition(float distance)
    {
        Vector3 landPosLocal = playerTransform.position - transform.position;
        landPosLocal.y = 0;
        landPosLocal.Normalize();
        landPosLocal *= distance;

        return transform.position + landPosLocal;
    }

    private IEnumerator SineJumpMovement(float height, float duration)
    {
        Vector3 startingPos = transform.position;
        Vector3 landingPosition = GetLandingPosition(jumpDistance);

        Debug.Log($"Starting jump with landing position {landingPosition}.");

        float distance = Vector3.Distance(transform.position, landingPosition);
        float invDst = 1.0f / (distance * distance);

        float t = 0.0f;
        float progress = 0.0f;
        float invDur = 1.0f / duration;

        while (progress < 1.0f)
        {
            progress = t * invDur;
            float remap = progress * distance;

            float x = Mathf.Lerp(startingPos.x, landingPosition.x, progress);
            float y = (4.0f * height * remap * (distance - remap)) * invDst;
            float z = Mathf.Lerp(startingPos.z, landingPosition.z, progress);

            transform.position = new(x, y, z);

            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        transform.position = landingPosition;
        yield return new WaitForSeconds(jumpDelay);
        _state.IsJumping = false;
    }
}
