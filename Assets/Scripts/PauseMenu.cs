using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public RectTransform panel;
    public float slideSpeed = 5f;

    private Vector2 hiddenPos;
    private Vector2 visiblePos;
    private bool isPaused = false;
    void Start()
    {
        visiblePos = panel.anchoredPosition;
        hiddenPos = visiblePos + new Vector2(0, 600); // move up

        panel.anchoredPosition = hiddenPos;
    }
    void Update()
    {
        if (isPaused)
        {
            panel.anchoredPosition = Vector2.Lerp(panel.anchoredPosition, visiblePos, Time.unscaledDeltaTime * slideSpeed);
        }
        else
        {
            panel.anchoredPosition = Vector2.Lerp(panel.anchoredPosition, hiddenPos, Time.unscaledDeltaTime * slideSpeed);
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }
}
