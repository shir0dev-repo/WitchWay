using UnityEngine;

public class MortarStuff : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) // trigger so it doesn't interfere with the crushing
    {
        if (other.TryGetComponent(out StateOfIngredient state))
        {
            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();

            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            state.ChangeIfInBowl(true);
            // changes the constraints instead of the kinematic so it still
            // generates collision stuff
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out StateOfIngredient state))
        {
            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();

            rigidbody.constraints = RigidbodyConstraints.None;
            state.ChangeIfInBowl(false);
        }
    }
}

