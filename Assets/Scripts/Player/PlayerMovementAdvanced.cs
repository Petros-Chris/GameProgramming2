using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementAdvanced : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float dashSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;
    public float groundDrag;
    public CameraMovement playerCamera;

    private Animator animator;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public int jumpCount;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;




    public Transform orientation;

    Vector3 moveDirection;
    Rigidbody rb;
    public MovementState state;
    public enum MovementState
    {
        frozen,
        walking,
        sprinting,
        crouching,
        sliding,
        dashing,
        air
    }

    public bool dashing;

    public bool sliding;

    [Header("Keybinds")]
    public FishGuard ass;
    private InputAction move;
    private InputAction jump;
    private InputAction sprint;
    private InputAction crouch;
    private InputAction look;
    public Vector2 walkMovement;

    void Awake()
    {
        ass = new FishGuard();
    }

    void OnEnable()
    {
        move = ass.Player.Move;
        jump = ass.Player.Jump;
        crouch = ass.Player.Crouch;
        sprint = ass.Player.Sprint;
        look = ass.Player.Look;

        move.Enable();
        jump.Enable();
        crouch.Enable();
        sprint.Enable();
        look.Enable();
    }

    void OnDisable()
    {
        move.Disable();
        jump.Disable();
        sprint.Disable();
        crouch.Disable();
        look.Disable();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerCamera = ComponentManager.Instance.playerCam.gameObject.GetComponent<CameraMovement>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        jumpCount = 0;
    }

    private void Update()
    {

        animator.SetFloat("CharacterSpeed", rb.velocity.magnitude);

        // Only process movement and jumping if the game is not paused or frozen
        if (GameMenu.Instance.playerFrozen)
        {
            walkMovement = Vector2.zero;
            return;
        }
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (grounded && state != MovementState.dashing)
        {
            rb.drag = groundDrag;
            jumpCount = 0;
        }
        else
            rb.drag = 0;

        playerCamera.DoFov(calculateFOV());



    }

    private void FixedUpdate()
    {
        // Only process movement and jumping if the game is not paused or frozen
        if (GameMenu.Instance.playerFrozen)
        {
            walkMovement = Vector2.zero;
            return;
        }
        MovePlayer();
    }



    private void MyInput()
    {
        walkMovement = move.ReadValue<Vector2>();


        // when to jump
        if (jump.triggered && readyToJump && jumpCount < 1)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (crouch.triggered)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (crouch.WasReleasedThisFrame())
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {

        // Mode - Dashing
        if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;

        }
        // Mode - Sliding
        else if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;

            else
                desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Crouching
        else if (crouch.IsPressed())
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && sprint.IsPressed())
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }

        // check if desiredMoveSpeed has changed drastically
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 100f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }


    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        if (state == MovementState.dashing)
            return;
        // calculate movement direction
        moveDirection = orientation.forward * walkMovement.y + orientation.right * walkMovement.x;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        if (jumpCount < 2)
        {
            // reset y velocity
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            jumpCount++;
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jumped");
        }
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private float calculateFOV()
    {
        float minFOV = 60f;
        float maxBaseFOV = 64f;
        float maxAbilityFOV = 75f;

        float fov = 60;

        if (moveSpeed <= 7f)
        {
            fov = Mathf.Lerp(minFOV, maxBaseFOV, 7 / 15f);

        }
        else
        {
            fov = Mathf.Lerp(maxBaseFOV, maxAbilityFOV, (moveSpeed - 7) / (15 - 7));

        }
        return fov;

    }


}