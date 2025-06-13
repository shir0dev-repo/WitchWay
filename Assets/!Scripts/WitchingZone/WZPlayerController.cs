using UnityEngine;
using UnityEngine.InputSystem;

public class WZPlayerController : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction lookAction;

    [Header("Tweaking")] //prob needs a better name
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float lookSpeed = 2.0f;

    //private vars
    //input
    private Vector2 moveInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;

    //refs
    private Rigidbody rb;
    private Transform camTransform;
    private float cameraPitch = 0.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        camTransform = GetComponentInChildren<Camera>().transform;

        //lock mouse
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable()
    {
        //enable input actions
        moveAction.Enable();
        lookAction.Enable();

        //subscibe to events
        moveAction.performed += SetMoveValues;
        moveAction.canceled += SetMoveValues;

        lookAction.performed += SetLookValues;
        lookAction.canceled += SetLookValues;
    }

    void OnDisable()
    {
        //unsubscribe
        moveAction.performed -= SetMoveValues;
        moveAction.canceled -= SetMoveValues;

        lookAction.performed -= SetLookValues;
        lookAction.canceled -= SetLookValues;

        //disable
        moveAction.Disable();
        lookAction.Disable();
    }

    void Update()
    {
        DoLook();
    }

    private void FixedUpdate()
    {
        DoMove();
    }

    //event for WASD control to update move value
    private void SetMoveValues(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    //event for mouse to update look values
    private void SetLookValues(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    //update players positon
    private void DoMove()
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
            rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void DoLook()
    {
        float mouseX = lookInput.x * lookSpeed * Time.deltaTime;
        float mouseY = lookInput.y * lookSpeed * Time.deltaTime;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, mouseX, 0f));

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        camTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}
