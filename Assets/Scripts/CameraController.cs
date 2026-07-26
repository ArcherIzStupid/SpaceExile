using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    void FixedUpdate()
    {
        if (player.position.x <= 0f)
        {
            transform.position = new Vector3(0f, player.position.y, -10f);
        }
        else
        {
            transform.position = new Vector3(player.position.x, player.position.y, -10f);
        }
    }
}
