using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ObjectText : MonoBehaviour
{
    public TextMeshProUGUI label;
    public TextMeshProUGUI readonlyText;

    public void Setup(
        string text,
        string value)
    {
        label.text = text;

        readonlyText.text = value;
    }
}
