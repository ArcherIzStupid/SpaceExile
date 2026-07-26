using UnityEngine;

public abstract class EditorHandle : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public abstract bool IsMouseOver();

    public abstract void BeginDrag();

    public abstract void Drag(Vector2 mouse);

    public abstract void EndDrag();
}