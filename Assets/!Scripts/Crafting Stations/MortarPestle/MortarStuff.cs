using UnityEngine;

public class MortarStuff : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ingredient")
        {
            Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();

            rigidbody.isKinematic = true;
            collision.collider.isTrigger = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ingredient")
        {
            Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();

            rigidbody.isKinematic = false;
            collision.collider.isTrigger = false;
        }
    }
}
