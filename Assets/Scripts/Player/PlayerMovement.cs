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
    public KeyCode jumpKey = KeyCode.Space;
    bool grounded;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    [Header("Dash")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    private bool isDashing;
    private float dashCooldown = 1f;
    private float dashTime;

    [Header("Stun")]
    public float stunDuration = 2f;
    private bool isStunned;
    private float stunTime;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        jumpCount = 0;
    }

    private void Update()
    {
        if (isStunned)
        {
            if (Time.time - stunTime >= stunDuration)
            {
                isStunned = false; // End stun after duration
            }
            return; // Skip movement if stunned
        }

        animator.SetFloat("CharacterSpeed", rb.velocity.magnitude);
        if (!GameMenu.playerFrozen)
        {
            MyInput();
            SpeedControl();
            isGrounded();
        }

        // Dash input handling
        if (Time.time >= dashTime + dashCooldown && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            MovePlayer();
        }
    }

    private void isGrounded()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
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

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

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
        if (jumpCount < 2)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset vertical velocity
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jumped");
        }
    }

    private void Dash()
    {
        if (!isDashing)
        {
            isDashing = true;
            dashTime = Time.time;
            Vector3 dashDirection = transform.forward;
            rb.velocity = new Vector3(dashDirection.x * dashSpeed, rb.velocity.y, dashDirection.z * dashSpeed);
            Invoke("EndDash", dashDuration);
        }
    }

    private void EndDash()
    {
        isDashing = false;
    }

    public void Stun()
    {
        if (!isStunned)
        {
            isStunned = true;
            stunTime = Time.time;
            rb.velocity = Vector3.zero; // Stop all movement during stun
            Debug.Log("Player is stunned");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyAI enemyComponent = collision.gameObject.GetComponent<EnemyAI>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(1, gameObject);
                Stun();
            }
        }
    }
}
