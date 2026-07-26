using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class SinglePlatformBehavior :
    MonoBehaviour,
    IEditableComponent,
    IEditableGizmo
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
            "Position X",
            transform.position.x,
            value =>
            {
                transform.position = new Vector3(
                    value,
                    transform.position.y,
                    transform.position.z);
            });

        panel.AddFloat(
            "Position Y",
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

    public void BuildGizmos(
        EditorGizmoManager manager)
    {
        manager.AddMoveHandle(transform);
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
}