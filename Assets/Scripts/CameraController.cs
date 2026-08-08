using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    void FixedUpdate()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -10f);
    }
}
