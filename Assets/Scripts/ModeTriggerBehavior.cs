using UnityEngine;

public class ModeTriggerBehavior : TriggerBehavior, IEditableComponent, IEditableGizmo
{
    public PlayerMode mode;

    public void ApplyValues()
    {
        
    }

    public void BuildEditor(ObjectInfoPanel panel)
    {
        
    }

    public void BuildGizmos(EditorGizmoManager manager)
    {
        throw new System.NotImplementedException();
    }

    public override void Execute(GameObject activator)
    {
        PlayerController.mode = mode;
    }

    public void HandleGizmos(EditorGizmoManager manager)
    {
        throw new System.NotImplementedException();
    }

    public void PullValues()
    {
        
    }
}