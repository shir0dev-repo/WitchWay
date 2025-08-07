using UnityEngine;

public class WZChaseAISight : MonoBehaviour
{
    private WZChaseAI monsterAI;
    private Transform playerTransform;
    [SerializeField] private LayerMask eyesightMask;
    [SerializeField] private float eyeHeight = 1.5f;

    private void Start()
    {
        monsterAI = GetComponentInParent<WZChaseAI>();
        playerTransform = WZPlayerManager.Instance?.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            monsterAI.OnPlayerEnteredSightCone();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!IsPlayerVisible())
            {
                monsterAI.OnPlayerLostSight();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            monsterAI.OnPlayerLostSight();
        }
    }

    private bool IsPlayerVisible()
    {
        if (playerTransform == null) return false;

        Vector3 origin = transform.position + Vector3.up;
        Vector3 target = playerTransform.position + Vector3.up;
        Vector3 direction = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);

        // If the ray hits *anything* in between that's not the player, it's blocked
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, eyesightMask))
        {
            Debug.DrawLine(origin, hit.point, Color.red, 1f);

            // Only count it as blocked if we hit something that is NOT the player
            if (!hit.transform.CompareTag("Player"))
            {
                Debug.Log("Sight blocked by: " + hit.transform.name);
                return false;
            }
        }

        // No obstruction OR directly hit the player
        Debug.DrawLine(origin, target, Color.green, 1f);
        return true;
    }
}
