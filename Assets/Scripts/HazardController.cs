using UnityEngine;

public class HazardController : MonoBehaviour
{
    public float damage = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player =
            other.GetComponent<PlayerController>();

        if (player == null)
            return;

        player.Respawn();
    }
}
