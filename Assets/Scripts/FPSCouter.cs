using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private float updateInterval = 0.5f; // Update UI every 0.5 seconds

    private float accumTime = 0f;
    private int frameCount = 0;

    void Update()
    {
        // Track the time passed and number of frames rendered
        accumTime += Time.unscaledDeltaTime;
        frameCount++;

        // Once the interval is reached, calculate and update the text
        if (accumTime >= updateInterval)
        {
            float fps = frameCount / accumTime;
            
            // Format to a whole number for cleaner UI performance
            fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";

            // Reset trackers for the next interval
            accumTime = 0f;
            frameCount = 0;
        }
    }
}
