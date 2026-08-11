using System.Diagnostics;
using UnityEngine;

public class GravityGearBehavior : MonoBehaviour, IEditableComponent, IEditableGizmo, IEditorToolCapabilities
{
    public GearController controller;
    public EditableObject editable;
    public PlayerController player;

    void Start()
    {
        editable = GetComponent<EditableObject>();
        player = GetComponent<PlayerController>();
    }

    public void BuildEditor(ObjectInfoPanel panel)
    {
        panel.AddField(
            "Object ID:",
            editable.objectID.ToString()
        );
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
            "Gravity Angle",
            player.Vector2Angle(PlayerController.gravityDirection),
            value =>
            {
                player.Vector2Angle(PlayerController.gravityDirection);
            }
        );
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
