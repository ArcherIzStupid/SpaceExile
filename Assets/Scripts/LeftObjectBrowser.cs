using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum EditorTab
{
    Ground,
    Hazards,
    Portals,
    Gears,
    Triggers,
    Collectibles,
    Decorations
}

[CreateAssetMenu(menuName = "Editor/Editor Object")]
public class EditorObject : ScriptableObject
{
    public string objectName;

    public Sprite icon;

    public GameObject prefab;

    public EditorTab tab;
}

public interface IEditableComponent
{
    void BuildEditor(ObjectInfoPanel panel);
}

public interface IEditableValue
{
    void PullValues();

    void ApplyValues();
}

public interface IEditableGizmo
{
    void BuildGizmos(EditorGizmoManager manager);
}

public class LeftObjectBrowser : MonoBehaviour
{
    public Transform content;

    public GameObject buttonPrefab;

    public TextMeshProUGUI title;

    public List<EditorObject> objects;

    public void ShowCategory(EditorTab tab)
    {
        title.text = tab.ToString();

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (EditorObject obj in objects)
        {
            if (obj.tab != tab)
                continue;

            GameObject button = Instantiate(buttonPrefab, content);

            button.GetComponent<ObjectButton>().Setup(obj);
        }

        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }
}