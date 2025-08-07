using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class WZChaseAI : MonoBehaviour
{
    [SerializeField] private WZChaseData _state;

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask eyesightMask;

    [Header("Idle Settings")]
    [SerializeField] private float idleWalk1 = 4f;
    [SerializeField] private float idleStop1 = 8f;
    [SerializeField] private float idleWalk2 = 8f;
    [SerializeField] private float idleStop2 = 14f;
    [SerializeField] private float idleIngredientDetectionRange = 12f;
    [SerializeField] private float idleSpeed = 1.5f;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolIngredientDetectionRange = 24f;
    [SerializeField] private float patrolPlayerDetectionRange = 12f;
    [SerializeField] private float patrolPointsToCheck = 3;
    [SerializeField] private float patrolSpeed = 2.5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackInterval = 5f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private int sanityDrainAmount = -1;
    [SerializeField] private int sanityTouchAmount = -2;
    [SerializeField] private float attackSpeed = 2f;

    [Header("Sound Settings")]
    [SerializeField] private float breathingSoundInterval = 12;

    private Vector3 patrolOrigin;

    private Coroutine patrolRoutine, idleRoutine;
    private float attackTimer = 0f;

    void Start()
    {
        _state = new WZChaseData(transform.position, transform.rotation);
        playerTransform = WZPlayerManager.Instance?.transform;
        TransitionToState(WZChaseData.State.Idle);

        GameEvents.WitchingZone.OnIngredientPickedUp += OnIngredientPickedUp;
    }

    void OnDestroy()
    {
        GameEvents.WitchingZone.OnIngredientPickedUp -= OnIngredientPickedUp;
    }

    void Update()
    {
        switch (_state.currentState)
        {
            case WZChaseData.State.Inactive:
                // wait for player to enter room
                break;
            case WZChaseData.State.Idle:
                HandleIdleState();
                break;

            case WZChaseData.State.Patrol:
                HandlePatrolState();
                break;

            case WZChaseData.State.Attack:
                HandleAttackState();
                break;
        }
    }

    public void TransitionToState(WZChaseData.State newState)
    {
        if (_state.currentState == newState) return;

        StopAllCoroutines();
        patrolRoutine = null;

        switch (newState)
        {
            case WZChaseData.State.Inactive:
                // Handle inactive state logic
                break;
            case WZChaseData.State.Idle:
                _state.IsWalking = false;
                _state.IsStopping = false;
                _state.IsBreathing = false;
                _state.IsGrunting = false;
                break;
            case WZChaseData.State.Patrol:
                _state.IsWalking = false;
                _state.IsStopping = false;
                _state.IsBreathing = false;
                Debug.Log("Grunt sound plays");
                break;
            case WZChaseData.State.Attack:
                _state.IsWalking = false;
                _state.IsStopping = false;
                _state.IsBreathing = false;
                _state.IsGrunting = false;
                attackTimer = 0f;
                break;
        }
        _state.currentState = newState;
        Debug.Log($"Transitioned to state: {_state.currentState}");
    }

    private void HandleIdleState()
    {
        if (!_state.IsBreathing)
        {
            StartCoroutine(PlayBreathingSound(breathingSoundInterval));
        }
        
        if (idleRoutine == null)
        {
            idleRoutine = StartCoroutine(IdleMovement());
        }
        
        if (IsPlayerInDetectionRange())
        {
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            TransitionToState(WZChaseData.State.Patrol);
        }
    }
    private IEnumerator IdleMovement()
{
    GameObject targetIngredient = FindClosestIngredient(idleIngredientDetectionRange);

    if (targetIngredient != null)
    {
        Debug.Log("Found nearby ingredient during Idle. Moving to it.");
        yield return StartCoroutine(MoveToPoint(targetIngredient.transform.position, idleSpeed));
        yield return StartCoroutine(StopForSeconds(idleStop1));
    }
    else
    {
        Debug.Log("No ingredient found nearby. Wandering randomly.");
        yield return StartCoroutine(WalkForSeconds(idleWalk1));
        yield return StartCoroutine(StopForSeconds(idleStop1));
        yield return StartCoroutine(WalkForSeconds(idleWalk2));
        yield return StartCoroutine(StopForSeconds(idleStop2));
    }

    idleRoutine = null;
}


    private void HandlePatrolState()
    {
        if (patrolRoutine == null)
        {
            patrolRoutine = StartCoroutine(PatrolState());
        }
    }

    private IEnumerator PatrolState()
    {

        yield return StartCoroutine(MoveToPoint(patrolOrigin, patrolSpeed));

        for (int i = 0; i < patrolPointsToCheck; i++)
        {
            yield return StartCoroutine(WalkAroundPointForSeconds(patrolOrigin, 3f));
            yield return StartCoroutine(LookAroundForSeconds(2f));
        }
        patrolRoutine = null;
        TransitionToState(WZChaseData.State.Idle);
    }
    private void HandleAttackState()
    {
        Vector3 toPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;
        Vector3 directionToPlayer = toPlayer.normalized;

        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = lookRotation;

        if (distanceToPlayer >= 1)
        {
            transform.position += directionToPlayer * attackSpeed * Time.deltaTime;
        }
        if (distanceToPlayer <= attackRange)
        {
            if (attackTimer <= 0f)
            {
                Debug.Log("Attack executed, draining sanity.");
                WZPlayerManager.Instance?.ModifySanity(sanityDrainAmount);
                attackTimer = attackInterval;
            }
            else
            {
                Debug.Log("Waiting for next attack, timer: " + attackTimer);
                attackTimer -= Time.deltaTime;
            }
        }
    }

    private IEnumerator WalkForSeconds(float seconds)
    {
        _state.IsWalking = true;
        float timer = 0f;
        Vector3 patrolTarget = Vector3.zero;
        bool foundTarget = false;
        float minDistance = 1.5f;

        for (int attempts = 0; attempts < 10 && !foundTarget; attempts++)
        {
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = 0;
            randomDir.Normalize();
            float distance = patrolSpeed * seconds;
            Vector3 candidate = transform.position + randomDir * distance;
            if (Vector3.Distance(transform.position, candidate) > minDistance &&
                !Physics.Raycast(transform.position, randomDir, distance, eyesightMask))
            {
                patrolTarget = candidate;
                foundTarget = true;
                Vector3 lookDir = playerTransform.position - transform.position;
            }
        }

        if (!foundTarget)
        {
            Debug.LogWarning("No valid patrol target found, exiting WalkForSeconds.");
            _state.IsWalking = false;
            yield break;
        }

        while (timer < seconds && Vector3.Distance(transform.position, patrolTarget) > 0.5f)
        {
            Vector3 dir = (patrolTarget - transform.position).normalized;
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 5f * Time.deltaTime);
            }

            transform.position += dir * patrolSpeed * Time.deltaTime;
            timer += Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator StopForSeconds(float seconds)
    {
        _state.IsStopping = true;
        float timer = 0f;
        float tiltAngle = -25f;

        while (timer < seconds)
        {
            Quaternion tilt = Quaternion.Euler(tiltAngle, transform.rotation.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, tilt, 2f * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        _state.IsStopping = false;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

    private IEnumerator WalkAroundPointForSeconds(Vector3 center, float seconds)
    {
        float timer = 0f;
        Vector3 patrolTarget = Vector3.zero;
        bool foundTarget = false;
        float minDistance = 1.5f;

        for (int attempts = 0; attempts < 10 && !foundTarget; attempts++)
        {
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = 0;
            randomDir.Normalize();
            float distance = patrolSpeed * seconds;
            Vector3 candidate = center + randomDir * distance;

            Vector3 toTarget = candidate - transform.position;
            float toTargetDist = toTarget.magnitude;
            if (Vector3.Distance(center, candidate) > minDistance &&
                !Physics.Raycast(transform.position, toTarget.normalized, toTargetDist, eyesightMask))
            {
                patrolTarget = candidate;
                foundTarget = true;
            }
        }

        if (!foundTarget)
            yield break;

        while (timer < seconds && Vector3.Distance(transform.position, patrolTarget) > 0.5f)
        {
            Vector3 dir = (patrolTarget - transform.position).normalized;
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 5f * Time.deltaTime);
            }

            transform.position += dir * patrolSpeed * Time.deltaTime;
            timer += Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator LookAroundForSeconds(float seconds)
    {
        float timer = 0f;
        float holdTime = 1.0f;
        float spinSpeed = 360f;

        while (timer < seconds)
        {
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = 0;
            if (randomDir == Vector3.zero) randomDir = transform.forward;
            randomDir.Normalize();

            float randomYaw = Quaternion.LookRotation(randomDir).eulerAngles.y;
            Quaternion targetRot = Quaternion.Euler(0f, randomYaw, 0f);

            float spinTimer = 0f;
            while (spinTimer < holdTime && timer < seconds)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, spinSpeed * Time.deltaTime);
                spinTimer += Time.deltaTime;
                timer += Time.deltaTime;

                if (!IsPlayerInDetectionRange())
                {
                    TransitionToState(WZChaseData.State.Idle);
                    yield break;
                }

                yield return null;
            }
        }
    }

    private IEnumerator MoveToPoint(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.5f)
        {
            Vector3 dir = (target - transform.position).normalized;
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 5f * Time.deltaTime);
            }

            transform.position += dir * speed * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator PlayBreathingSound(float interval)
    {
        _state.IsBreathing = true;
        yield return new WaitForSeconds(interval);
        if (_state.currentState == WZChaseData.State.Idle)
        {
            Debug.Log("Breathing sound plays");
        }
        _state.IsBreathing = false;
    }

    private bool IsPlayerInDetectionRange()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) <= patrolPlayerDetectionRange)
        {
            patrolOrigin = playerTransform.position;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnPlayerEnteredSightCone()
    {
        if (_state.currentState != WZChaseData.State.Attack)
        {
            Debug.Log("Player entered sight cone");
            TransitionToState(WZChaseData.State.Attack);
        }
    }
    public void OnPlayerLostSight()
    {
        if (_state.currentState == WZChaseData.State.Attack)
        {
            Debug.Log("Player lost sight, transitioning to Patrol state.");
            patrolOrigin = playerTransform.position;
            TransitionToState(WZChaseData.State.Patrol);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            EatNearbyIngredients();
            Debug.Log("Ingredient eaten.");
        }
        else if (other.CompareTag("Player"))
        {
            WZPlayerManager.Instance?.ModifySanity(sanityTouchAmount);
        }
    }
    private GameObject FindClosestIngredient(float range)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);
        GameObject closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Ingredient"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closest = hit.gameObject;
                    closestDist = dist;
                }
            }
        }

        return closest;
    }


    private void EatNearbyIngredients()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 3f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Ingredient"))
            {
                Destroy(hit.gameObject);
            }
        }
    }
    private void OnIngredientPickedUp(Vector3 position)
    {
        if (Vector3.Distance(transform.position, position) <= patrolIngredientDetectionRange && _state.currentState == WZChaseData.State.Idle)
        {
            patrolOrigin = position;
            TransitionToState(WZChaseData.State.Patrol);
            Debug.Log("Ingredient picked up nearby, transitioning to Patrol state.");
        }
    }
}