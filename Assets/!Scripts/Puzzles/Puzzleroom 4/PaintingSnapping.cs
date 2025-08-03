using UnityEngine;

public class PaintingSnapping : MonoBehaviour
{
    bool _canSnap = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canSnap = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canSnap = false;
        }
    }
}
