using UnityEngine;

public class MoveHandle : EditorHandle
{
    private enum MoveAxis
    {
        None,
        X,
        Y,
        Both
    }

    [Header("Hitboxes")]
    [Tooltip("Collider covering the right-pointing blue arrow.")]
    [SerializeField] private Collider2D xAxisCollider;

    [Tooltip("Collider covering the upward-pointing red arrow.")]
    [SerializeField] private Collider2D yAxisCollider;

    [Tooltip("Optional collider covering the center hub for free movement.")]
    [SerializeField] private Collider2D centerCollider;

    [Header("Screen Size")]
    [Tooltip("Camera size at which the gizmo uses its normal prefab scale.")]
    [SerializeField] private float referenceCameraSize = 5f;

    [SerializeField] private float gizmoScale = 1f;

    [Header("Hierarchy")]
    [SerializeField] private Transform gizmoRoot;

    private Transform target;
    private Camera editorCamera;

    private MoveAxis hoveredAxis;
    private MoveAxis draggingAxis;

    private Vector2 dragStartMouse;
    private Vector3 dragStartPosition;

    public void Initialize(
        Transform newTarget,
        Camera newEditorCamera)
    {
        target = newTarget;
        editorCamera = newEditorCamera;

        if (target != null)
            transform.position = target.position;
    }

    public override bool IsMouseOver(Vector2 mouseWorld)
    {
        hoveredAxis = MoveAxis.None;

        // Check the center first because its collider may overlap
        // the inner ends of the arrows.
        if (centerCollider != null &&
            centerCollider.OverlapPoint(mouseWorld))
        {
            hoveredAxis = MoveAxis.Both;
            return true;
        }

        if (xAxisCollider != null &&
            xAxisCollider.OverlapPoint(mouseWorld))
        {
            hoveredAxis = MoveAxis.X;
            return true;
        }

        if (yAxisCollider != null &&
            yAxisCollider.OverlapPoint(mouseWorld))
        {
            hoveredAxis = MoveAxis.Y;
            return true;
        }

        return false;
    }

    public override void BeginDrag(Vector2 mouseWorld)
    {
        if (target == null || hoveredAxis == MoveAxis.None)
            return;

        draggingAxis = hoveredAxis;
        dragStartMouse = mouseWorld;
        dragStartPosition = target.position;
    }

    public override void Drag(Vector2 mouseWorld)
    {
        if (target == null || draggingAxis == MoveAxis.None)
            return;

        Vector2 mouseDelta =
            mouseWorld - dragStartMouse;

        Vector3 newPosition =
            dragStartPosition;

        switch (draggingAxis)
        {
            case MoveAxis.X:
                newPosition.x =
                    dragStartPosition.x +
                    mouseDelta.x;
                break;

            case MoveAxis.Y:
                newPosition.y =
                    dragStartPosition.y +
                    mouseDelta.y;
                break;

            case MoveAxis.Both:
                newPosition.x =
                    dragStartPosition.x +
                    mouseDelta.x;

                newPosition.y =
                    dragStartPosition.y +
                    mouseDelta.y;
                break;
        }

        target.position = newPosition;
        transform.position = newPosition;
    }

    public override void EndDrag()
    {
        draggingAxis = MoveAxis.None;
        hoveredAxis = MoveAxis.None;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Follow the object when its position changes through the info panel.
        transform.position = target.position;

        // Maintain roughly the same on-screen size while zooming.
        if (editorCamera != null &&
            editorCamera.orthographic &&
            referenceCameraSize > 0f)
        {
            float zoomScale =
                editorCamera.orthographicSize /
                referenceCameraSize;

            if (gizmoRoot != null)
            {
                gizmoRoot.localScale =
                    Vector3.one *
                    gizmoScale *
                    zoomScale;
            }
        }

        Physics2D.SyncTransforms();
    }
}