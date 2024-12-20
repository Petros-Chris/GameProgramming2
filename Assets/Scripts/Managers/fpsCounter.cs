using UnityEngine;
using TMPro;

public class FramerateCounter : MonoBehaviour
{
    [Tooltip("Delay between updates of the displayed framerate value")]
    public float pollingTime = 0.5f;
    [Tooltip("The text field displaying the framerate")]
    public TextMeshProUGUI uiText;

    float m_AccumulatedDeltaTime = 0f;
    int m_AccumulatedFrameCount = 0;

    void Update()
    {
        if (uiText == null)
        {
            if (Settings.Instance.fpsGuiInstance != null)
            {
                uiText = Settings.Instance.fpsGuiInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            }
        }

        m_AccumulatedDeltaTime += Time.deltaTime;
        m_AccumulatedFrameCount++;

        if (m_AccumulatedDeltaTime >= pollingTime)
        {
            int framerate = Mathf.RoundToInt((float)m_AccumulatedFrameCount / m_AccumulatedDeltaTime);
            uiText.text = framerate.ToString();

            m_AccumulatedDeltaTime = 0f;
            m_AccumulatedFrameCount = 0;
        }
    }
}