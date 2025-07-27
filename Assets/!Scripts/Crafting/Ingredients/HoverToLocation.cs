using System;
using UnityEngine;


public class HoverToLocation : MonoBehaviour
{
    [SerializeField] private float _hoverForce = 12.0f;

    public Transform Target { get; set; } = null;
    private Rigidbody _rigidbody;

    private bool _isHovering = false;

    private void OnEnable()
    {
        GameEvents.Crafting.OnObjectAttachedToCursor += DisableHover;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Target != null)
            HoverToTarget();
    }

    private void HoverToTarget()
    {
        if (_rigidbody == null) return;

        Vector3 toTarget = Target.position - transform.position;
        float dist = toTarget.magnitude;
        toTarget /= dist;

        Vector3 force = Time.fixedDeltaTime * _hoverForce * toTarget;
        
        _rigidbody.AddForce(force, ForceMode.VelocityChange);
        _isHovering = dist <= 0.1f;
        _rigidbody.useGravity = !_isHovering;
    }

    private void DisableHover(IFollowCursor cursorObj)
    {
        if (cursorObj is not WorldIngredient wIng) return;
        if (GetComponent<WorldIngredient>() != wIng) return;

        if (_isHovering)
        {
            CursorManager.Instance.ClearCursor(false);
        }
    }
}
