using UnityEngine;

public class WZChaseAISight : MonoBehaviour
{
    private WZChaseAI monsterAI;
    private Transform playerTransform;
    [SerializeField] private LayerMask eyesightMask;
    [SerializeField] private float eyeHeight = 1.5f;
    [SerializeField] private bool debugVision = false;

    private void Start()
    {
        monsterAI = GetComponentInParent<WZChaseAI>();
        playerTransform = WZPlayerManager.Instance?.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && IsPlayerVisible())
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

        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 target = playerTransform.position + Vector3.up * eyeHeight;
        Vector3 direction = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, eyesightMask))
        {
            if (debugVision)
                Debug.DrawLine(origin, hit.point, Color.red, 1f);

            if (!hit.transform.CompareTag("Player"))
            {
                if (debugVision)
                    Debug.Log("Sight blocked by: " + hit.transform.name);
                return false;
            }
        }

        if (debugVision)
            Debug.DrawLine(origin, target, Color.green, 1f);
        return true;
    }
}
