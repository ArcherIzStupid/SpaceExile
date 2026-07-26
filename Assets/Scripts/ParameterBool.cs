using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ParameterBool : MonoBehaviour
{
    public TextMeshProUGUI label;

    public Toggle toggle;

    public void Setup(
        string text,
        bool value,
        UnityAction<bool> callback)
    {
        label.text = text;

        toggle.isOn = value;

        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(callback);
    }
}
