using UnityEngine;
using UnityEngine.Events;

public class ObjectInfoPanel : MonoBehaviour
{
    [Header("Container")]
    public Transform content;

    [Header("Parameter Prefabs")]
    public GameObject floatParameterPrefab;
    public GameObject boolParameterPrefab;
    public GameObject dropdownParameterPrefab;

    public void ShowObject(EditableObject obj)
    {
        Clear();
    
        foreach (IEditableComponent component
                 in obj.GetComponents<IEditableComponent>())
        {
            component.BuildEditor(this);
        }
    
        EditorGizmoManager.instance.ShowGizmos(obj);
    }

    public void Clear()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddFloat(string label, float value, UnityAction<float> callback)
    {
        GameObject field =
            Instantiate(
                floatParameterPrefab,
                content);

        field
            .GetComponent<ParameterFloat>()
            .Setup(
                label,
                value,
                callback);
    }

    public ParameterBool AddBool()
    {
        GameObject field =
            Instantiate(
                boolParameterPrefab,
                content
            );

        return field.GetComponent<ParameterBool>();
    }

    public ParameterDropdown AddDropdown()
    {
        GameObject field =
            Instantiate(
                dropdownParameterPrefab,
                content
            );

        return field.GetComponent<ParameterDropdown>();
    }
}