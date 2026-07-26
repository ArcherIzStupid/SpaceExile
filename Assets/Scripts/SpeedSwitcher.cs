using UnityEngine;

public class SpeedSwitcher : MonoBehaviour
{
    [Header("Speed")]

    public PlayerSpeed speedToSwitchTo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerController.speed = speedToSwitchTo;

            Debug.Log("Switched to" + speedToSwitchTo + "speed");
        }
    }
}
