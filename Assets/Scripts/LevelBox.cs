using UnityEngine;

public class LevelBox : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player =
            other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.Respawn();
        }
    }
}

