using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ParameterFloat : MonoBehaviour
{
    public TextMeshProUGUI label;

    public TMP_InputField input;

    public void Setup(
        string text,
        float value,
        UnityAction<float> callback)
    {
        label.text = text;

        input.text = value.ToString();

        input.onEndEdit.RemoveAllListeners();

        input.onEndEdit.AddListener(s =>
        {
            if (float.TryParse(s, out float result))
            {
                callback(result);
            }
        });
    }
}