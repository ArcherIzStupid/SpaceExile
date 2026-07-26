using UnityEngine;
using IngameDebugConsole;
using JetBrains.Annotations;
using Unity.Android.Gradle.Manifest;
using UnityEditor.Rendering;
using System.Drawing;
using Microsoft.VisualBasic;

public class Commands : MonoBehaviour
{

    //==================================================
    // MODE
    //==================================================

    [ConsoleMethod("mode", "Changes player's mode")]
    public static void ChangeMode(PlayerMode mode)
    {
        PlayerController.mode = mode;

        Debug.Log("Mode Changed to " + mode);
    }

    //==================================================
    // SIZE
    //==================================================

    [ConsoleMethod("size", "Changes player size")]
    public static void ChangeSize(PlayerSize size)
    {
        PlayerController.size = size;

        Debug.Log("Size changed to " + size);
    }

    //==================================================
    // GRAVITY
    //==================================================

    [ConsoleMethod("gravity", "set player's gravity vector")]
    public static void UpdateGravityVector(Vector2 dir)
    {
        PlayerController.gravityDirection = dir;


        Debug.Log("Gravtiy updated, current gravity" + PlayerController.gravityDirection);
    }

    //==================================================
    // SPEED
    //==================================================
    
    [ConsoleMethod("speed", "Sets the speed of the player")]
    public static void ChangeSpeed(PlayerSpeed speed)
    {
        PlayerController.speed = speed;

        Debug.Log("Player speed changed to" + speed);
    }
}
