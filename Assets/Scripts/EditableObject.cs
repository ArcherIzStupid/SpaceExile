using UnityEngine;

public class EditableObject : MonoBehaviour
{
    public EditorObject editorObject;

    public int objectID = -1;
    public int groupID = -1;

    private void Start()
    {
        if (EditorObjectRegistry.instance != null)
        {
            EditorObjectRegistry.instance
                .Register(this);
        }
    }

    private void OnEnable()
    {
        if (EditorObjectRegistry.instance != null)
        {
            EditorObjectRegistry.instance
                .Register(this);
        }
    }

    private void OnDisable()
    {
        if (EditorObjectRegistry.instance != null)
        {
            EditorObjectRegistry.instance
                .Unregister(this);
        }
    }
}