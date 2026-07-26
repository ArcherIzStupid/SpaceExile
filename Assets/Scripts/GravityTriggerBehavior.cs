using UnityEngine;

public class GravityTriggerBehavior : TriggerBehavior, IEditableComponent, IEditableGizmo
{
    public float positionX;
    public float postiionY;

    public float Scale;
    [Range(0,360)]
    public float gravityAngle;

    public bool rotateVelocity;

    public override void Execute(GameObject activator)
    {
        Debug.Log("Trigger Activated");
        PlayerController player =
            activator.GetComponent<PlayerController>();

        if (player == null)
            return;

        float radians =
            gravityAngle * Mathf.Deg2Rad;

        Vector2 direction =
            new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            ).normalized;

        player.SetGravity(direction);

        if (rotateVelocity)
        {
            //player.RotateVelocity(direction);
        }
    }

    public void BuildEditor(ObjectInfoPanel panel)
    {
        panel.AddFloat(
            "X Position",
            positionX,
            value => positionX = value
        );

        panel.AddFloat(
            "Y Position",
            postiionY,
            value => postiionY = value
        );

        panel.AddFloat(
            "Gravity Angle",
            gravityAngle,
            value => gravityAngle = value
        );

        panel.AddBool().Setup(
            "Rotate Velocity",
            rotateVelocity,
            value => rotateVelocity = value
        );
    }

    public void BuildGizmos(EditorGizmoManager manager)
    {
        manager.AddMoveHandle(gameObject.transform);
    }
}
