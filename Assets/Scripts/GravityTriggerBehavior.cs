using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class GravityTriggerBehavior : TriggerBehavior, IEditableComponent, IEditableGizmo
{
    public float positionX;
    public float postiionY;

    public float Scale;
    [Range(0,360)]
    public float gravityAngle;

    public PlayerController player;
    public bool invertControls = false;

    void Start()
    {
        player = GetComponent<PlayerController>();
    }

    public override void Execute(GameObject activator)
    {
        PlayerController player =
            activator.GetComponent<PlayerController>();

        if (player == null)
            return;

        float radians =
            gravityAngle * Mathf.Deg2Rad + 90;

        Vector2 direction =
            new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            ).normalized;

        player.SetGravity(direction);
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
            "Invert Controls",
            invertControls,
            value => invertControls = value
        );
    }

    public void BuildGizmos(EditorGizmoManager manager)
    {
        manager.AddMoveHandle(gameObject.transform);
    }
}
