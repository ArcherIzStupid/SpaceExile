using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System;

public class ParameterDropdown : MonoBehaviour
{
    public TextMeshProUGUI label;

    public TMP_Dropdown dropdown;

    public void Setup<T>(
        string text,
        T value,
        UnityAction<T> callback)
        where T : Enum
    {
        label.text = text;

        dropdown.ClearOptions();

        dropdown.AddOptions(
            new System.Collections.Generic.List<string>(
                Enum.GetNames(typeof(T))
            ));

        dropdown.value =
            Array.IndexOf(
                Enum.GetValues(typeof(T)),
                value);

        dropdown.onValueChanged.RemoveAllListeners();

        dropdown.onValueChanged.AddListener(i =>
        {
            callback((T)Enum.GetValues(typeof(T)).GetValue(i));
        });
    }
}