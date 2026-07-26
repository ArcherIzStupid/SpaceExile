using System;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using NUnit.Framework;
using JetBrains.Annotations;
using System.Runtime.InteropServices;

public class ModeSwitcher : MonoBehaviour
{
    [Header("Mode")]

    public PlayerMode modeToSwitchTo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerController.mode = modeToSwitchTo;

            Debug.Log(
                "Switched to: " + modeToSwitchTo
            );
        }
    }
}
