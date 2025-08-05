using UnityEngine;

public class PaintingSnapping : MonoBehaviour
{
    [SerializeField] WZInteractable interact;
    [SerializeField] string hint; // for debugging purposes

    bool _canPickUp = false;
    public bool _isInCorrectPosition = false;

    private void Update()
    {
        if (!_canPickUp) { return; }

        if (Input.GetKeyDown(KeyCode.E))
        {
            interact.Interacted();
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
