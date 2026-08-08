using UnityEngine;
using UnityEngine.UI;

public class EditorToolBar : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button moveButton;
    [SerializeField] private Button rotateButton;
    [SerializeField] private Button scaleButton;

    [Header("Optional Selected Visuals")]
    [SerializeField] private GameObject moveSelected;
    [SerializeField] private GameObject rotateSelected;
    [SerializeField] private GameObject scaleSelected;

    private EditableObject selectedObject;

    private void Awake()
    {
        moveButton.onClick.AddListener(
            () => SelectTool(EditorTool.Move));

        rotateButton.onClick.AddListener(
            () => SelectTool(EditorTool.Rotate));

        scaleButton.onClick.AddListener(
            () => SelectTool(EditorTool.Scale));
    }

    public void SetSelectedObject(EditableObject editable)
    {
        selectedObject = editable;

        EditorGizmoManager.instance
            .SetSelectedObject(editable);

        RefreshButtons();
    }

    public void ClearSelectedObject()
    {
        selectedObject = null;
    
        EditorGizmoManager.instance
            .ClearSelectedObject();
    
        RefreshButtons();
    }

    public void SelectTool(EditorTool tool)
    {
        if (!CanUseTool(tool))
            return;

        EditorGizmoManager.instance.SetTool(tool);

        RefreshSelectedVisuals(tool);
    }

    private void RefreshButtons()
    {
        moveButton.interactable =
            CanUseTool(EditorTool.Move);

        rotateButton.interactable =
            CanUseTool(EditorTool.Rotate);

        scaleButton.interactable =
            CanUseTool(EditorTool.Scale);

        EditorTool activeTool =
            EditorGizmoManager.instance.CurrentTool;

        if (!CanUseTool(activeTool))
        {
            activeTool = EditorTool.None;
        }

        RefreshSelectedVisuals(activeTool);
    }

    private bool CanUseTool(EditorTool tool)
    {
        if (selectedObject == null)
            return false;

        IEditorToolCapabilities[] capabilities =
            selectedObject.GetComponents<IEditorToolCapabilities>();

        foreach (IEditorToolCapabilities capability
                 in capabilities)
        {
            if (capability.CanUseTool(tool))
                return true;
        }

        return false;
    }

    private EditorTool GetFirstAvailableTool()
    {
        if (CanUseTool(EditorTool.Move))
            return EditorTool.Move;

        if (CanUseTool(EditorTool.Rotate))
            return EditorTool.Rotate;

        if (CanUseTool(EditorTool.Scale))
            return EditorTool.Scale;

        return EditorTool.None;
    }

    private void RefreshSelectedVisuals(EditorTool tool)
    {
        if (moveSelected != null)
            moveSelected.SetActive(
                tool == EditorTool.Move);

        if (rotateSelected != null)
            rotateSelected.SetActive(
                tool == EditorTool.Rotate);

        if (scaleSelected != null)
            scaleSelected.SetActive(
                tool == EditorTool.Scale);
    }
}