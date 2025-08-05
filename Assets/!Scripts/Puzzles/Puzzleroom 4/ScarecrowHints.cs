using UnityEngine;

public class ScarecrowHints : MonoBehaviour
{
    [SerializeField] string _hint;
    bool _isPlayerInRange = false;

    private void Update()
    {
        if (!_isPlayerInRange) { return; }

        if (Input.GetKeyDown(KeyCode.Space)) { Debug.Log(_hint); }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) { _isPlayerInRange = true; }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) { _isPlayerInRange = false; }
    }
}
