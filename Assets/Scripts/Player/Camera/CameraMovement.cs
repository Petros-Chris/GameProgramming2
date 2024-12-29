using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
public class CameraMovement : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;
    public Transform mesh;
    float xRotation;
    float yRotation;
    public InputAction look;
    FishGuard ass;
    public Vector2 cameraMovement;

    void Awake()
    {
        ass = new FishGuard();
    }
    void OnEnable()
    {
        look = ass.Player.Look;
        look.Enable();
    }

    void OnDisable()
    {
        look.Disable();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (orientation == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            orientation = playerObj.transform.Find("Orientation").gameObject.transform;
            mesh = playerObj.transform.Find("Incidental 70").gameObject.transform;
        }
        // Locks camera in first person
        if (GameMenu.Instance.isPaused)
        {
            return;
        }

        if (GameMenu.Instance.isInGameMenuOpen)
        {
            return;
        }
        cameraMovement = look.ReadValue<Vector2>();
        float mouseXAxis = cameraMovement.x * Time.deltaTime * sensX;
        float mouseYAxis = cameraMovement.y * Time.deltaTime * sensY;

        yRotation += mouseXAxis;
        xRotation -= mouseYAxis;
        //Prevents player from breaking their neck
        xRotation = Mathf.Clamp(xRotation, -80f, 60f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        if (orientation != null)
        {
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
            // I think this is getting hit with gimbal lock :O
            mesh.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);

    }
}
