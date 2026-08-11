using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class SinglePlatformBehavior : MonoBehaviour, IEditableComponent, IEditableGizmo, IEditorToolCapabilities
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    #region Editor

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
            "Width",
            spriteRenderer.size.x,
            SetWidth);

        panel.AddFloat(
            "Height",
            spriteRenderer.size.y,
            SetHeight);

        panel.AddFloat(
            "Rotation",
            transform.eulerAngles.z,
            value =>
            {
                transform.rotation =
                    Quaternion.Euler(
                        0,
                        0,
                        value);
            });
    }

    public void BuildGizmos(EditorGizmoManager manager)
    {
        switch (manager.CurrentTool)
        {
            case EditorTool.Move:
                manager.AddMoveHandle(transform);
                break;
    
            case EditorTool.Rotate:
                // manager.AddRotateHandle(transform);
                break;
    
            case EditorTool.Scale:
                // manager.AddScaleHandle(this);
                break;
        }
    }

    #endregion

    private void SetWidth(float width)
    {
        spriteRenderer.size =
            new Vector2(
                width,
                spriteRenderer.size.y);

        boxCollider.size =
            new Vector2(
                width,
                boxCollider.size.y);
    }

    private void SetHeight(float height)
    {
        spriteRenderer.size =
            new Vector2(
                spriteRenderer.size.x,
                height);

        boxCollider.size =
            new Vector2(
                boxCollider.size.x,
                height);
    }

    public bool CanUseTool(EditorTool tool)
    {
        switch (tool)
        {
            case EditorTool.Move:
            case EditorTool.Rotate:
            case EditorTool.Scale:
                return true;

            default:
                return false;
        }
    }
}