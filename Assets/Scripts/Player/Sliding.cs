using UnityEngine;
using UnityEngine.InputSystem;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovementAdvanced pm;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    private bool slide;
    public InputAction slideKey;
    FishGuard fishGuardMovement;
    void Awake()
    {
        fishGuardMovement = new FishGuard();
    }
    void OnEnable()
    {
        slideKey = fishGuardMovement.Player.Slide;
        slideKey.Enable();
    }

    void OnDisable()
    {
        slideKey.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovementAdvanced>();

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        if (slideKey.IsPressed() && (pm.walkMovement.x != 0 || pm.walkMovement.y != 0) && pm.grounded && !slide)
        {
            StartSlide();
            slide = true;
        }
        if (slideKey.WasPressedThisFrame())
            slide = false;
        if (slideKey.WasReleasedThisFrame() && pm.sliding)
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        pm.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = pm.orientation.forward * pm.walkMovement.y + pm.orientation.right * pm.walkMovement.x;

        // sliding normal
        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        // sliding down a slope
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
            StopSlide();
    }

    private void StopSlide()
    {
        pm.sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
