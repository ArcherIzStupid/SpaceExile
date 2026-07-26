using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectButton : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI objectName;

    private EditorObject editorObject;

    Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(SelectObject);
    }

    public void Setup(EditorObject obj)
    {
        editorObject = obj;

        icon.sprite = obj.icon;

        objectName.text = obj.objectName;
    }

    public void SelectObject()
    {
        Debug.Log("Selected " + editorObject.objectName);

        EditorManager.instance.SelectObject(editorObject);
    }
}