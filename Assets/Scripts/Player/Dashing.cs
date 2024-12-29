using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMovementAdvanced pm;

    [Header("Dash")]
    public float dashForce = 15;
    public float dashUpForce = 0;
    public float dashDuration = 2;

    public float dashCooldown = 1.5f;
    private float dashCooldownTimer;
    private InputAction dashKey;
    public bool disableGravity = true;
    FishGuard fishGuardMovement;
    void Awake()
    {
        fishGuardMovement = new FishGuard();
    }
    void OnEnable()
    {
        dashKey = fishGuardMovement.Player.Dash;
        dashKey.Enable();
    }

    void OnDisable()
    {
        dashKey.Disable();
    }

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovementAdvanced>();
    }

    private void Update()
    {
        if (dashKey.triggered && !GameMenu.Instance.playerFrozen)
        {
            Dash();
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }


    private void Dash()
    {
        if (dashCooldownTimer > 0)
        {
            return;
        }
        else
        {
            dashCooldownTimer = dashCooldown;
        }
        pm.dashing = true;

        Transform forwardT = pm.orientation;


        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + pm.orientation.up * dashUpForce;

        if (disableGravity)
            rb.useGravity = false;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);

    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        pm.dashing = false;

        if (disableGravity)
        {
            rb.useGravity = true;
        }
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        Vector3 direction = new Vector3();

        direction = forwardT.forward * pm.walkMovement.y + forwardT.right * pm.walkMovement.x;

        if (pm.walkMovement.y == 0 && pm.walkMovement.x == 0)
        {
            direction = forwardT.forward;
        }
        return direction.normalized;
    }
}