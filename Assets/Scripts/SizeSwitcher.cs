using System.Drawing;
using UnityEngine;
using IngameDebugConsole;

public class SizeSwitcher : MonoBehaviour
{
    [Header("Size")]

    public PlayerSize size;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        { 
            PlayerController.size = size;

            Debug.Log(
                "Switched to:" + size
            );
        }
    }
}
