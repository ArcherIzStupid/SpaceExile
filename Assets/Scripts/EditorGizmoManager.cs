using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorGizmoManager : MonoBehaviour
{
    public static EditorGizmoManager instance;

    public Camera editorCamera;

    private readonly List<EditorHandle> handles =
        new();

    private EditorHandle selectedHandle;

    void Awake()
    {
        instance = this;
    }

    public void Clear()
    {
        handles.Clear();
        selectedHandle = null;
    }

    public void AddHandle(EditorHandle handle)
    {
        handles.Add(handle);
    }

    public void ShowGizmos(EditableObject obj)
    {
        Clear();

        foreach (IEditableGizmo gizmo
            in obj.GetComponents<IEditableGizmo>())
        {
            gizmo.BuildGizmos(this);
        }
    }

    void Update()
    {
        if (!EditorManager.editorOpen)
            return;

        Vector2 mouse =
            editorCamera.ScreenToWorldPoint(
                Mouse.current.position.ReadValue());

        if (selectedHandle == null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                foreach (EditorHandle handle in handles)
                {
                    if (handle.IsMouseOver())
                    {
                        selectedHandle = handle;

                        handle.BeginDrag();

                        break;
                    }
                }
            }
        }
        else
        {
            if (Mouse.current.leftButton.isPressed)
            {
                selectedHandle.Drag(mouse);
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                selectedHandle.EndDrag();

                selectedHandle = null;
            }
        }
    }

    public MoveHandle moveHandlePrefab;
    
    public MoveHandle AddMoveHandle(Transform target)
    {
        MoveHandle handle =
            Instantiate(
                moveHandlePrefab,
                transform);

        handles.Add(handle);

        return handle;
    }
}