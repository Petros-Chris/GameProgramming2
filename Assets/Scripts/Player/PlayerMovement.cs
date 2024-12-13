using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float airMultiplier;
    public float playerHeight;
    public LayerMask whatIsGround;
    private Animator animator;

    private int jumpCount;
    //private bool readyToJump;
    public KeyCode jumpKey = KeyCode.Space;
    bool grounded;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        jumpCount = 0;
    }

    private void Update()
    {
        animator.SetFloat("CharacterSpeed", rb.velocity.magnitude);
        // Only process movement and jumping if the game is not paused or frozen
        if (GameMenu.playerFrozen)
        {
            horizontalInput = 0;
            verticalInput = 0;
            return;
        }

        MyInput();
        SpeedControl();
        isGrounded();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void isGrounded()
    {
        // Check if the player is grounded using a raycast
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        // Reset the jump count when the player touches the ground
        if (grounded)
        {
            rb.drag = groundDrag;
            jumpCount = 0;
        }
        else
        {
            rb.drag = 0;
        }
    }

    /// <summary>
    /// Handles player input (movement and jumping)
    /// </summary>
    private void MyInput()
    {
        // Get movement input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump input handling
        if (Input.GetKeyDown(jumpKey) && jumpCount < 1)
        {
            Jump();
            jumpCount++;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    /// <summary>
    /// Prevents player from exceeding max speed
    /// </summary>
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // Only allow jump if jumpCount < 2 (ensures no third jump)
        if (jumpCount < 2)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset vertical velocity before jumping
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jumped");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collisions, for example with enemies
        if (collision.gameObject.TryGetComponent<EnemyAI>(out EnemyAI enemyComponent))
        {
            enemyComponent.TakeDamage(1, gameObject);
        }
    }
}
