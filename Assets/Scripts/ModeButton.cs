using UnityEngine;

public class ModeButton : MonoBehaviour
{
    public EditorMode mode;

    public EditorManager manager;

    public void OnClick()
    {
        manager.SwitchEditorMode(mode);
    }
}
