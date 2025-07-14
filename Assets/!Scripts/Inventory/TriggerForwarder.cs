using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForwarder : MonoBehaviour
{
    [SerializeField] private bool passColliderData = true;

    //collider data passing functions
    public Action<Collider> onTriggerEnter;
    public Action<Collider> onTriggerExit;
    public Action<Collider> onTriggerStay;

    //regular functions
    public Action onTriggerEnterNorm;
    public Action onTriggerExitNorm;
    public Action onTriggerStayNorm;

    //tracking for multi collider objects
    private HashSet<int> pickedUp = new HashSet<int>();

    void OnTriggerEnter(Collider collision)
    {
        int id = collision.gameObject.GetInstanceID();
        if (pickedUp.Contains(id)) return;
        pickedUp.Add(id);

        if (passColliderData) onTriggerEnter?.Invoke(collision);
        else onTriggerEnterNorm?.Invoke();
    }

    void OnTriggerExit(Collider collision)
    {
        if (passColliderData) onTriggerExit?.Invoke(collision);
        else onTriggerExitNorm?.Invoke();
    }

    void OnTriggerStay(Collider collision)
    {
        if (passColliderData) onTriggerStay?.Invoke(collision);
        else onTriggerStayNorm?.Invoke();
    }
}
