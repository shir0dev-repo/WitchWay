using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/*
Possible improvements:
    - jump buffering
*/

//this script is big may be good to split it

public enum PlayerControllerActions //for disabling specfic actions
{
    moveAction,
    lookAction,
    jumpAction,
    crouchAction
}

public class WZPlayerController : MonoBehaviour
{
    [Header("World Controls")]
    [SerializeField] private InputAction moveAction;
    [Space(5)]
    [SerializeField] private InputAction lookAction;
    [Space(5)]
    [SerializeField] private InputAction jumpAction;
    [Space(5)]
    [SerializeField] private InputAction crouchAction;

    [Header("Move and Look values")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float lookSpeed = 2.0f;

    [Header("Jump Vars")]
    [SerializeField] private float jumpPower = 10;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private float groundDist = 0.4f;

    [Header("Crouch Vars")]
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float percentOrigHeight;

    [Header("Loading")]
    [SerializeField] private int exitPortalSceneIndex;
    [SerializeField] private TriggerForwarder wzExitPortal;

    //private vars
    //input
    private Vector2 moveInput = Vector2.zero;
    [HideInInspector] public Vector2 lookInput = Vector2.zero;
    private float currentSpeed;

    //refs
    private Rigidbody rb;
    private Transform camTransform;
    private float cameraPitch = 0.0f;

    private Vector3 originalScale;
    private bool wasGrounded = false;

    private bool canMove = true;

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody>();
        camTransform = GetComponentInChildren<Camera>().transform;

        originalScale = transform.localScale;

        //lock mouse
        Cursor.lockState = CursorLockMode.Locked;

        currentSpeed = moveSpeed;

        wasGrounded = Physics.CheckSphere(groundCheckPos.position, groundDist, groundLayer);

        if (wzExitPortal != null) wzExitPortal.onTriggerEnterNorm += () => LoadScene(exitPortalSceneIndex); //change to collider data passer if its allowed to collide with things other thatn player
    }

    void OnEnable()
    {
        //enable input actions
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        crouchAction.Enable();

        //subscibe to events
        moveAction.performed += SetMoveValues;
        moveAction.canceled += SetMoveValues;

        lookAction.performed += SetLookValues;
        lookAction.canceled += SetLookValues;

        jumpAction.performed += DoJump;

        crouchAction.performed += DoCrouch;
        crouchAction.canceled += UnDoCrouch;
    }

    void OnDisable()
    {
        //unsubscribe
        moveAction.performed -= SetMoveValues;
        moveAction.canceled -= SetMoveValues;

        lookAction.performed -= SetLookValues;
        lookAction.canceled -= SetLookValues;

        jumpAction.performed -= DoJump;

        crouchAction.performed -= DoCrouch;
        crouchAction.canceled -= UnDoCrouch;

        //disable
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        crouchAction.Disable();
    }

    void Update()
    {
        DoLook();
    }

    private void FixedUpdate()
    {
        DoMove();

        //grounded check
        bool isGrounded = Physics.CheckSphere(groundCheckPos.position, groundDist, groundLayer);
        if (!wasGrounded && isGrounded)
        {
            if (crouchAction.inProgress) ApplyCrouch(); //if holding crouch when landing, crouch
        }
        wasGrounded = isGrounded;
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
            rb.MovePosition(rb.position + move * currentSpeed * Time.fixedDeltaTime);
        }
    }

    //use mouse input to move camera
    private void DoLook()
    {
        float mouseX = lookInput.x * lookSpeed * Time.deltaTime;
        float mouseY = lookInput.y * lookSpeed * Time.deltaTime;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, mouseX, 0f));

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        camTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    //jump when space pressed
    private void DoJump(InputAction.CallbackContext context)
    {
        if (Physics.CheckSphere(groundCheckPos.position, groundDist, groundLayer))
        {
            transform.localScale = originalScale;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
        }
    }

    //crouch button held
    private void DoCrouch(InputAction.CallbackContext context)
    {
        if (Physics.CheckSphere(groundCheckPos.position, groundDist, groundLayer))
        {
            ApplyCrouch();
        }
    }

    //crouch button released
    private void UnDoCrouch(InputAction.CallbackContext context)
    {
        currentSpeed = moveSpeed;

        transform.localScale = originalScale;
    }

    //should be changed many to anim in future or really just a camera offset
    private void ApplyCrouch()
    {
        currentSpeed = crouchSpeed;

        float scalePercent = percentOrigHeight / 100f;

        Vector3 newScale = new Vector3(originalScale.x, originalScale.y * scalePercent, originalScale.z);
        transform.localScale = newScale;

        //set player position to the ground
        transform.position = new Vector3(transform.position.x, transform.position.y - newScale.y, transform.position.z);
    }

    //load scene
    private void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex); //add a loading anim and load async
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
        if (canMove)
        {
            moveAction.Enable();
            lookAction.Enable();
            jumpAction.Enable();
            crouchAction.Enable();
        }
        else
        {
            moveAction.Disable();
            lookAction.Disable();
            jumpAction.Disable();
            crouchAction.Disable();
        }
    }

    public void EnableDisableAction(bool enabled, PlayerControllerActions[] actions)
    {
        foreach (PlayerControllerActions action in actions)
        {
            InputAction targetAction = null;
            switch (action)
            {
                case PlayerControllerActions.moveAction:
                    targetAction = moveAction;
                    break;

                case PlayerControllerActions.lookAction:
                    targetAction = lookAction;
                    break;

                case PlayerControllerActions.jumpAction:
                    targetAction = jumpAction;
                    break;

                case PlayerControllerActions.crouchAction:
                    targetAction = crouchAction;
                    break;
            }
            
            if (targetAction == null) continue;

            if (enabled) targetAction.Enable();
            else targetAction.Disable();
        }
    }

    //debugging
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckPos.position, groundDist); //visualize ground check
    }
#endif
}
