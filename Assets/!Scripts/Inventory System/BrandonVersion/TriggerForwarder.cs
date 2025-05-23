using System;
using UnityEngine;

public class TriggerForwarder : MonoBehaviour
{
    public Action<Collider> onTriggerEnter;

    void OnTriggerEnter(Collider collision)
    {
        onTriggerEnter?.Invoke(collision);
    }
}
