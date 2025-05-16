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
}
