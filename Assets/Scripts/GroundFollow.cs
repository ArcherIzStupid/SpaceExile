using System.Drawing;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using IngameDebugConsole;

public class GroundFollow : MonoBehaviour
{
    public Transform player;

    public PlayerSize size;
    public Vector2 gravity;

    public Vector3 smallOffset;
    public Vector3 offset;
    public Vector3 bigOffset;
    public void LateUpdate()
    {
        size = PlayerController.size;
        gravity = PlayerController.gravityDirection;
        
        switch(size)
        {
            case PlayerSize.Small:
                transform.position = player.position + smallOffset;
                return;
            case PlayerSize.Normal:
                transform.position = player.position + offset;
                return;
            case PlayerSize.Big:
                transform.position = player.position + bigOffset;
                return;
        }
    }
}
