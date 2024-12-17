using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{
    [Header ("References")]
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMovementAdvanced pm;


     [Header("Dash")]
    public float dashForce;
    public float dashUpForce;
    public float dashDuration;

    public float dashCooldown;
    private float dashCooldownTimer;
    
    public KeyCode dashKey = KeyCode.E;

    public bool disableGravity = true;


    public void Start(){
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovementAdvanced>();
    }
    private void Update(){
         if (Input.GetKeyDown(KeyCode.E))
        {
            Dash();
        }

        if(dashCooldownTimer > 0){
            dashCooldownTimer -= Time.deltaTime;
        }

    }


    private void Dash()
    {
        if(dashCooldownTimer > 0){
            return;
        }
        else{
            dashCooldownTimer = dashCooldown;
        }
        pm.dashing = true;

        Transform forwardT = orientation;


        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpForce;

        if(disableGravity)
            rb.useGravity = false;
        
        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);

    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce(){

        

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }
       
        private void ResetDash()
    {
        pm.dashing = false;

        if(disableGravity){
            rb.useGravity = true;
        }
    }

    private Vector3 GetDirection(Transform forwardT){
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        Vector3 direction = new Vector3();

        direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;

        if(verticalInput == 0 && horizontalInput == 0){

            direction = forwardT.forward;
        }
        return direction.normalized;
    }

}