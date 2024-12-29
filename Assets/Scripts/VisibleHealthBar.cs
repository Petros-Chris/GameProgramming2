using UnityEngine;
using UnityEngine.UI;

public class VisibleHealthBar : MonoBehaviour
{
    private Slider slider;
    private Camera cam;
    private Transform target;
    private bool camWasChanged;
    public Vector3 offSet;
    public int warningHealthPercent = 60, badHealthPercent = 30;
    private float warningHealth, badHealth;

    private void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
        UpdateHealthBar(100, 100); // Default initializer
    }

    public void Start()
    {
        cam = ComponentManager.Instance.playerCam;
        target = gameObject.transform.parent;
        warningHealth = warningHealthPercent / 100f;
        badHealth = badHealthPercent / 100f;
    }

    private void Update()
    {
        HealthBarLookAt();
        // Makes the health bar look at the camera
        transform.SetPositionAndRotation(target.position + offSet, cam.transform.rotation);
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
        // More than warning health
        if (slider.value > warningHealth)
        {
            slider.fillRect.GetComponent<Image>().color = Color.green;
        }
        // Less than warning but more than bad health
        if (slider.value <= warningHealth && slider.value > badHealth)
        {
            slider.fillRect.GetComponent<Image>().color = Color.yellow;
        }
        // Less than bad health
        else if (slider.value <= badHealth)
        {
            slider.fillRect.GetComponent<Image>().color = Color.red;
        }
    }

    private void HealthBarLookAt()
    {
        // If false, it means it is in player mode
        if (!camWasChanged)
        {
            // If player Is alive
            if (ComponentManager.Instance.playerCam.gameObject.activeInHierarchy)
                return;

            // If player Is dead
            if (ComponentManager.Instance.deathCam.gameObject.activeInHierarchy)
            {
                cam = ComponentManager.Instance.deathCam;
                camWasChanged = true;
                return;
            }
            // If player Is building
            if (ComponentManager.Instance.buildCam.gameObject.activeInHierarchy)
            {
                cam = ComponentManager.Instance.buildCam;
                camWasChanged = true;
                return;
            }
        }
        // If true, it means it is not in player mode
        if (camWasChanged)
        {
            // If not in player mode
            if (!ComponentManager.Instance.playerCam.gameObject.activeInHierarchy)
                return;

            cam = ComponentManager.Instance.playerCam;
            camWasChanged = false;
        }
    }
}