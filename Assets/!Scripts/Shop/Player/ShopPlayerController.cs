using UnityEngine;
using UnityEngine.InputSystem;

public class ShopPlayerController : MonoBehaviour
{
    public enum InputMode { Locked = 0, Freed = 1, Dialogue = 2 }

    [SerializeField] private LayerMask _groundLayer;

    [Header("Locomotion")]
    [SerializeField] private Vector2 _moveSpeed = Vector3.one * 5.0f;
    [SerializeField] private Rigidbody2D _rigidbody;
    [Header("Interaction")]
    [SerializeField] private Transform _interactPosition;
    [SerializeField] private float _interactRadius = 0.5f;
    [Space]
    [SerializeField] private InputAction _moveAction;
    [SerializeField] private InputAction _interactAction;
    [SerializeField] private InputAction _advanceDialogueAction;

    private Vector2 _inputThisFrame = Vector3.zero;

    private void OnEnable()
    {
        _interactAction.started += TryInteract;
        ToggleInput(InputMode.Freed);
    }

    private void OnDisable()
    {
        _interactAction.started -= TryInteract;
        ToggleInput(InputMode.Locked);
    }

    private void Update()
    {
        RegisterInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void RegisterInput()
    {
        if (_moveAction.enabled && _moveAction.IsPressed())
            _inputThisFrame = _moveAction.ReadValue<Vector2>().normalized;
        else
            _inputThisFrame = Vector2.zero;
    }

    private void HandleMovement()
    {
        if (_inputThisFrame == Vector2.zero) return;

        Vector2 moveDir = Time.deltaTime * Vector2.Scale(_inputThisFrame, _moveSpeed);

        if (_interactPosition != null)
            _interactPosition.localPosition = _inputThisFrame;

        _rigidbody.MovePosition(transform.position + new Vector3(moveDir.x, moveDir.y));
    }

    private void TryInteract(InputAction.CallbackContext context)
    {
        Collider2D[] c = Physics2D.OverlapCircleAll((Vector2)_interactPosition.position, _interactRadius);

        foreach (Collider2D c1 in c)
        {
            if (c1.TryGetComponent(out IInteractable interactable))
            {
                // if we successfully interacted, break out of this loop
                if (interactable.Interact(this))
                    return;
            }
        }
    }

    public void ToggleInput(InputMode mode)
    {
        switch (mode)
        {
            case InputMode.Locked:
                _moveAction.Disable();
                _interactAction.Disable();
                _advanceDialogueAction.Disable();
                break;
            case InputMode.Dialogue:
                _moveAction.Disable();
                _interactAction.Disable();
                _advanceDialogueAction.Enable();
                break;
            case InputMode.Freed:
            default:
                _moveAction.Enable();
                _interactAction.Enable();
                _advanceDialogueAction.Disable();
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_interactPosition == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_interactPosition.position, _interactRadius);
    }
}
