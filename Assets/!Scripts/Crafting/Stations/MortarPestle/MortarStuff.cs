using UnityEngine;

public class MortarStuff : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) // trigger so it doesn't interfere with the crushing
    {
        if (other.gameObject.tag == "Ingredient")
        {
            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();

            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            // changes the constraints instead of the kinematic so it still
            // generates collision stuff
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ingredient")
        {
            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();

            rigidbody.constraints = RigidbodyConstraints.None;
        }
    }
}

