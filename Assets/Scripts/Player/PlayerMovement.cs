using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [Header("Keybinds")]
    [Header("Ground Check")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public int maxJumps = 2;

    private int jumpCount;
    bool readyToJump;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode switchToBuildMode = KeyCode.M;
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    public Camera buildCam;
    public Camera playerCam;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        readyToJump = true;
        jumpCount = maxJumps;
    }

    private void Update()
    {
        if (Input.GetKeyDown(switchToBuildMode))
        {
            GameMenu.playerFrozen = true;
            playerCam.gameObject.SetActive(false);
            buildCam.gameObject.SetActive(true);
        }

        if (!GameMenu.playerFrozen)
        {
            MyInput();
            SpeedControl();
            isGrounded();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void isGrounded()
    {
        // Checks if the player is on the ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        // Changes how the movement feels depending on if it is flying
        if (grounded)
        {
            rb.drag = groundDrag;
            jumpCount = maxJumps; // Reset jump count when grounded
        }
        else
        {
            rb.drag = 0;
        }
    }

    /// <summary>
    /// Gets the input to control the player
    /// </summary>
    private void MyInput()
    {
        // Get inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jumping input with double-jump logic
        if (Input.GetKeyDown(jumpKey) && readyToJump && jumpCount > 0)
        {
            readyToJump = false;
            Jump();
            jumpCount--; // Decrease jump count on each jump

            // When the cooldown has been reached, it allows the user to jump again
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Changes player physics while off the ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    /// <summary>
    /// Ensures the player doesn't go past the set moveSpeed
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
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<EnemyAI>(out EnemyAI enemyComponent))
        {
            enemyComponent.TakeDamage(1);
        }
    }
}