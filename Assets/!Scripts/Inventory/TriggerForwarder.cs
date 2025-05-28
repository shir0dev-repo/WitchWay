using System;
using UnityEngine;

public class TriggerForwarder : MonoBehaviour
{
    public Action<Collider> onTriggerEnter;
    public Action<Collider> onTriggerExit;
    public Action<Collider> onTriggerStay;

    void OnTriggerEnter(Collider collision)
    {
        onTriggerEnter?.Invoke(collision);
    }

    void OnTriggerExit(Collider collision)
    {
        onTriggerExit?.Invoke(collision);
    }

    void OnTriggerStay(Collider collision)
    {
        onTriggerStay?.Invoke(collision);
    }
}
