using UnityEngine;

public class UnderwaterEffects : MonoBehaviour
{
    public Color underwaterFogColor = new Color(0.2f, 0.4f, 0.6f, 1f);
    public float underwaterFogDensity = 0.02f;
    public Color underwaterAmbientLight = new Color(0.1f, 0.2f, 0.3f, 1f);

    private Color defaultFogColor;
    private float defaultFogDensity;
    private Color defaultAmbientLight;
    private bool isUnderwater;

    void Start()
    {
        // Save default settings
        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;
        defaultAmbientLight = RenderSettings.ambientLight;

        // Enable fog
        RenderSettings.fog = true;
    }

    void Update()
    {
        if (transform.position.y < 0) // Assume water surface is at Y = 0
        {
            if (!isUnderwater)
            {
                EnterUnderwater();
            }
        }
        else
        {
            if (isUnderwater)
            {
                ExitUnderwater();
            }
        }
    }

    void EnterUnderwater()
    {
        RenderSettings.fogColor = underwaterFogColor;
        RenderSettings.fogDensity = underwaterFogDensity;
        RenderSettings.ambientLight = underwaterAmbientLight;
        isUnderwater = true;
    }

    void ExitUnderwater()
    {
        RenderSettings.fogColor = defaultFogColor;
        RenderSettings.fogDensity = defaultFogDensity;
        RenderSettings.ambientLight = defaultAmbientLight;
        isUnderwater = false;
    }
}
