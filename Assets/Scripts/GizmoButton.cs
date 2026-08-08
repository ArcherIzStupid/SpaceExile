using UnityEngine;

public class GizmoButton : MonoBehaviour
{
    public EditorToolBar toolBar;
    public EditorTool tool;

    public void ChangeGizmo()
    {
        toolBar.SelectTool(tool);
    }
}
