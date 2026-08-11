using UnityEngine;

public class JumpGearBehavior : MonoBehaviour, IEditableComponent, IEditableGizmo, IEditorToolCapabilities
{
    public GearController controller;

    void Start()
    {
        controller = GetComponent<GearController>();
    }
    public void BuildEditor(ObjectInfoPanel panel)
    {
        panel.AddFloat(
            "X Position",
            transform.position.x,
            value =>
            {
                transform.position = new Vector3(
                    value,
                    transform.position.y,
                    transform.position.z);
            });

        panel.AddFloat(
            "Y Position",
            transform.position.y,
            value =>
            {
                transform.position = new Vector3(
                    transform.position.x,
                    value,
                    transform.position.z);
            });
        
        panel.AddFloat(
            "Scale",
            transform.localScale.x,
            value =>
            {
                transform.localScale = new Vector3(
                    value,
                    value,
                    transform.localScale.z);
            });

        panel.AddFloat(
            "Jump Force",
            controller.gearForce,
            value =>
            {
                controller.gearForce = value;
            });
    }

    public void BuildGizmos(EditorGizmoManager manager)
    {
        switch (manager.CurrentTool)
        {
            case EditorTool.Move:
                manager.AddMoveHandle(transform);
                break;
    
            case EditorTool.Scale:
                // manager.AddScaleHandle(this);
                break;
        }
    }

    public bool CanUseTool(EditorTool tool)
    {
        switch (tool)
        {
            case EditorTool.Move:
            case EditorTool.Scale:
                return true;

            default:
                return false;
        }
    }
}
