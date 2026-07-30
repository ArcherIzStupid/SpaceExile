using UnityEngine;

/// <summary>
/// Base class for every in-game editor handle.
/// Input is controlled by EditorGizmoManager.
/// </summary>
public abstract class EditorHandle : MonoBehaviour
{
    /// <summary>
    /// Returns true when the mouse is over an interactive part of this handle.
    /// </summary>
    public abstract bool IsMouseOver(Vector2 mouseWorld);

    /// <summary>
    /// Called once when dragging starts.
    /// </summary>
    public abstract void BeginDrag(Vector2 mouseWorld);

    /// <summary>
    /// Called every frame while dragging.
    /// </summary>
    public abstract void Drag(Vector2 mouseWorld);

    /// <summary>
    /// Called once when the mouse button is released.
    /// </summary>
    public abstract void EndDrag();
}