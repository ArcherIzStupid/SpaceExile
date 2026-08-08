using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public enum EditorMode
{
    Build,
    Edit
}

public class LevelEditor : MonoBehaviour
{
    public Camera editorCamera;

    public float gridSize = 1f;

    public float previewAlpha = 0.5f;

    public GameObject previewObject;

    public ObjectInfoPanel infoPanel;

    private GameObject selectedPlacedObject;

    private Vector2 movement;

    public float cameraSpeed = 10f;

    [SerializeField] EditorToolBar toolBar;

    [Header("Navigation")]

    public float zoomSpeed = 50f;
    public float minZoom = 2f;
    public float maxZoom = 20f;

    public float panSpeed = 1f;

    private Vector2 lastMousePosition;

    void Update()
    {
        if (!EditorManager.editorOpen)
        {
            DestroyPreview();
            return;
        }

        if(EditorManager.instance.currentMode
            == EditorMode.Build)
        {
            HandlePreview();

            HandlePlacement();

            HandleDelete();
        }

        if(EditorManager.instance.currentMode
            == EditorMode.Edit)
        {
            HandleSelection();
        }

        HandleEditorMovement();

        if(EditorManager.instance.currentMode == EditorMode.Edit)
        {
            DestroyPreview();
        }

        HandleCameraPan();

        HandleCameraZoom();
    }

    public void HandleEditorMovement()
    {
        float moveX =
            Keyboard.current.dKey.isPressed ? 1 :
            Keyboard.current.aKey.isPressed ? -1 : 0;

        float moveY =
            Keyboard.current.wKey.isPressed ? 1 :
            Keyboard.current.sKey.isPressed ? -1 : 0;

        movement = new Vector2(
            moveX,
            moveY
        );

        editorCamera.transform.Translate(
            movement *
            cameraSpeed *
            Time.unscaledDeltaTime,

            Space.World
        );
    }

    public void CreatePreview()
    {
        DestroyPreview();

        if (EditorManager.instance.selectedObject == null)
            return;

        previewObject = Instantiate(
            EditorManager.instance.selectedObject.prefab
        );

        previewObject.name = "Preview";

        SetPreviewAppearance(previewObject);
    }

    void DestroyPreview()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
    }

    void HandlePreview()
    {
        if (previewObject == null)
            return;

        if(!EditorManager.editorOpen)
        {
            DestroyPreview();
            return;
        }

        Vector3 mouseScreen =
            Mouse.current.position.ReadValue();

        mouseScreen.z =
            Mathf.Abs(
                editorCamera.transform.position.z
            );

        Vector2 mousePos =
            editorCamera.ScreenToWorldPoint(
                mouseScreen
            );

        previewObject.transform.position =
            SnapPosition(mousePos);
    }

    void HandlePlacement()
    {
        if (EditorManager.instance.selectedObject == null)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Vector2 mousePos =
            editorCamera.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );

        Vector2 position =
            SnapPosition(mousePos);

        Instantiate(
            EditorManager.instance.selectedObject.prefab,

            position,

            Quaternion.identity
        );
    }

    void HandleDelete()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!Mouse.current.rightButton.wasPressedThisFrame)
            return;
        if (!Mouse.current.rightButton.wasPressedThisFrame)
            return;

        Vector3 mouseScreen =
            Mouse.current.position.ReadValue();

        mouseScreen.z =
            Mathf.Abs(
                editorCamera.transform.position.z
            );

        Vector2 mousePos =
            editorCamera.ScreenToWorldPoint(
                mouseScreen
            );

        Collider2D hit =
            Physics2D.OverlapPoint(mousePos);

        if (hit == null)
            return;

        if (hit.gameObject == previewObject)
            return;

        Destroy(hit.gameObject);
    }

    Vector2 SnapPosition(Vector2 pos)
    {
        return new Vector2(
            Mathf.Round(pos.x / gridSize) * gridSize,

            Mathf.Round(pos.y / gridSize) * gridSize
        );
    }

    void SetPreviewAppearance(GameObject obj)
    {
        SpriteRenderer[] renderers =
            obj.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in renderers)
        {
            Color c = sr.color;

            c.a = previewAlpha;

            sr.color = c;
        }

        Collider2D[] colliders =
            obj.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
    }

    void HandleSelection()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;
        
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if(EditorManager.instance.currentMode != EditorMode.Edit)
            return;

        Vector2 mousePos = editorCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        EditorGizmoManager gizmos =
            EditorGizmoManager.instance;

        if (gizmos != null &&
            (gizmos.IsInteracting ||
             gizmos.IsPointerOverAnyHandle(mousePos)))
        {
            return;
        }

        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if(hit == null)
            return;

        selectedPlacedObject = hit.gameObject;

        EditableObject editable =
            hit.GetComponent<EditableObject>();
        
        if (editable == null)
            return;
        
        infoPanel.ShowObject(editable);

        toolBar.SetSelectedObject(editable);
    }

    public void HandleCameraPan()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Mouse.current.middleButton.wasPressedThisFrame)
        {
            lastMousePosition =
                Mouse.current.position.ReadValue();
    
            return;
        }
    
        if (!Mouse.current.middleButton.isPressed)
            return;
    
        Vector2 currentMouse =
            Mouse.current.position.ReadValue();
    
        Vector2 delta =
            currentMouse - lastMousePosition;
    
        float unitsPerPixel =
            editorCamera.orthographicSize * 2f / Screen.height;
    
        editorCamera.transform.position -=
            new Vector3(
                delta.x * unitsPerPixel,
                delta.y * unitsPerPixel,
                0f
            );
    
        lastMousePosition = currentMouse;
    }

    public void HandleCameraZoom()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        float scroll =
            Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) < 0.01f)
            return;

        Camera cam = editorCamera;

        Vector3 mouseScreen =
            Mouse.current.position.ReadValue();

        mouseScreen.z =
            Mathf.Abs(cam.transform.position.z);

        Vector3 beforeZoom =
            cam.ScreenToWorldPoint(mouseScreen);

        cam.orthographicSize -=
            scroll * zoomSpeed * Time.unscaledDeltaTime;

        cam.orthographicSize =
            Mathf.Clamp(
                cam.orthographicSize,
                minZoom,
                maxZoom);

        Vector3 afterZoom =
            cam.ScreenToWorldPoint(mouseScreen);

        cam.transform.position +=
            beforeZoom - afterZoom;
    }
}