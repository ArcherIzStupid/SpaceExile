using CitrioN.Common;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorManager : MonoBehaviour
{
    public static EditorManager instance;

    [Header("Current Selection")]

    public EditorTab currentTab;

    public EditorObject selectedObject;

    [Header("References")]

    public LeftObjectBrowser objectBrowser;
    public LevelEditor levelEditor;
    public Canvas canvas;

    [Header("States")]

    public static bool editorOpen;

    [Header("Editor Mode")]

    public EditorMode currentMode = EditorMode.Build;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeTab(EditorTab tab)
    {
        currentTab = tab;

        objectBrowser.ShowCategory(tab);
    }

    public void SelectObject(EditorObject obj)
    {
        selectedObject = obj;

        Debug.Log("Selected " + obj.objectName);

        levelEditor.CreatePreview();
    }

    void Update()
    {
        // 1. Check for the Tab key press
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            editorOpen = !editorOpen;
            ShowEditor(); // 2. Call ShowEditor immediately after toggling
        }
    }

    public void ShowEditor()
    {
        // 3. Toggles the UI Canvas
        if (canvas != null)
        {
            canvas.enabled = editorOpen;
            if(editorOpen)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }

    public void SwitchEditorMode(EditorMode mode)
    {
        currentMode = mode;
    }
}