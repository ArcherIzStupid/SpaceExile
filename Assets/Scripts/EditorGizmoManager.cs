using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EditorTool
{
    None,
    Move,
    Rotate,
    Scale
}

public interface IEditorToolCapabilities
{
    bool CanUseTool(EditorTool tool);
}

public class EditorGizmoManager : MonoBehaviour
{
    public static EditorGizmoManager instance;

    [Header("References")]
    [SerializeField] private Camera editorCamera;
    [SerializeField] private Transform gizmoContainer;

    [Header("Handle Prefabs")]
    [SerializeField] private MoveHandle moveHandlePrefab;

    private readonly List<EditorHandle> activeHandles =
        new List<EditorHandle>();

    private EditorHandle draggingHandle;

    public bool IsInteracting =>
        draggingHandle != null;

        private EditableObject selectedObject;

        public EditorTool CurrentTool { get; private set; }
            = EditorTool.Move;

    private void Awake()
    {
        if (instance != null &&
            instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (gizmoContainer == null)
            gizmoContainer = transform;
    }

    private void Update()
    {
        if (!EditorManager.editorOpen)
            return;

        if (Mouse.current == null ||
            editorCamera == null)
            return;

        Vector2 mouseWorld =
            GetMouseWorldPosition();

        if (draggingHandle == null)
        {
            if (Mouse.current.leftButton
                .wasPressedThisFrame)
            {
                TryBeginDrag(mouseWorld);
            }

            return;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            draggingHandle.Drag(mouseWorld);
        }

        if (Mouse.current.leftButton
            .wasReleasedThisFrame)
        {
            draggingHandle.EndDrag();
            draggingHandle = null;
        }
    }

    private void TryBeginDrag(Vector2 mouseWorld)
    {
        // Reverse order makes the most recently added handle
        // take priority if handles overlap.
        for (int i = activeHandles.Count - 1;
             i >= 0;
             i--)
        {
            EditorHandle handle =
                activeHandles[i];

            if (handle == null)
                continue;

            if (!handle.IsMouseOver(mouseWorld))
                continue;

            draggingHandle = handle;
            draggingHandle.BeginDrag(mouseWorld);
            return;
        }
    }

    public void ShowGizmos(EditableObject editable)
    {
        Clear();

        if (editable == null)
            return;

        IEditableGizmo[] gizmoComponents =
            editable.GetComponents<IEditableGizmo>();

        foreach (IEditableGizmo component
                 in gizmoComponents)
        {
            component.BuildGizmos(this);
        }
    }

    public MoveHandle AddMoveHandle(
        Transform target)
    {
        if (moveHandlePrefab == null)
        {
            Debug.LogError(
                "Move Handle Prefab is not assigned " +
                "on EditorGizmoManager.");

            return null;
        }

        MoveHandle handle =
            Instantiate(
                moveHandlePrefab,
                gizmoContainer);

        handle.Initialize(
            target,
            editorCamera);

        activeHandles.Add(handle);

        return handle;
    }

    public bool IsPointerOverAnyHandle(
        Vector2 mouseWorld)
    {
        foreach (EditorHandle handle
                 in activeHandles)
        {
            if (handle != null &&
                handle.IsMouseOver(mouseWorld))
            {
                return true;
            }
        }

        return false;
    }

    public void Clear()
    {
        if (draggingHandle != null)
        {
            draggingHandle.EndDrag();
            draggingHandle = null;
        }

        foreach (EditorHandle handle
                 in activeHandles)
        {
            if (handle != null)
                Destroy(handle.gameObject);
        }

        activeHandles.Clear();
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector2 mouseScreen =
            Mouse.current.position.ReadValue();

        Vector3 world =
            editorCamera.ScreenToWorldPoint(
                new Vector3(
                    mouseScreen.x,
                    mouseScreen.y,
                    0f));

        return new Vector2(
            world.x,
            world.y);
    }

    public void SetSelectedObject(
        EditableObject editable)
    {
        selectedObject = editable;
        RebuildCurrentGizmo();
    }

    public void SetTool(EditorTool tool)
    {
        CurrentTool = tool;
        RebuildCurrentGizmo();
    }

    private void RebuildCurrentGizmo()
    {
        Clear();
    
        if (selectedObject == null ||
            CurrentTool == EditorTool.None)
        {
            return;
        }
    
        IEditorToolCapabilities[] capabilities =
            selectedObject
                .GetComponents<IEditorToolCapabilities>();
    
        bool allowed = false;
    
        foreach (IEditorToolCapabilities capability
                 in capabilities)
        {
            if (capability.CanUseTool(CurrentTool))
            {
                allowed = true;
                break;
            }
        }
    
        if (!allowed)
            return;
    
        IEditableGizmo[] gizmoComponents =
            selectedObject.GetComponents<IEditableGizmo>();
    
        foreach (IEditableGizmo gizmo
                 in gizmoComponents)
        {
            gizmo.BuildGizmos(this);
        }
    }
}