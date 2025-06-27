using System;
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

    void OnTriggerEnter(Collider collision)
    {
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
