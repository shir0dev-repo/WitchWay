using Unity.VisualScripting;
using UnityEngine;

public class PaintingSnapping : MonoBehaviour
{
    [SerializeField] string hint; // for debugging purposes

    bool _canPickUp = false;
    public bool _isInCorrectPosition = false;

    private void Update()
    {
        if (!_canPickUp) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(hint);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canPickUp = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canPickUp = false;
        }
    }

}
