using UnityEngine;

public class SpeedTriggerBehavior : TriggerBehavior
{
    public PlayerSpeed speed;

    public override void Execute(GameObject activator)
    {
        PlayerController player =
            activator.GetComponent<PlayerController>();

        if(player == null)
            return;

        
    }
}
