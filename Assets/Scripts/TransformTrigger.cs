using UnityEngine;

public class TransformTrigger : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float triggerDelay;
    public Vector2 endPos;
    public float endRotation;
    public Vector3 endScale = Vector3.one;
    public Vector3 activatePos;
    public int activateType;
    private Vector3 error0;
    private float error1;
    private float error2;
    private bool alreadyActivated;
    private float activationTimer;

    [Header("Player Transform")]
    public Transform player;

    void Update()
    {
        if (player == null)
        {
            PlayerController foundPlayer =
                FindFirstObjectByType<PlayerController>();

            if (foundPlayer == null)
                return;

            player = foundPlayer.transform;
        }

        error0 = activatePos - player.position;
        error1 = activatePos.x - player.position.x;
        error2 = activatePos.y - player.position.y;

        bool shouldActivate =
            activateType == 0 &&
            error0.sqrMagnitude < new Vector3(0.1f, 0.1f, 0).sqrMagnitude;

        shouldActivate =
            shouldActivate ||
            (activateType == 1 && error1 < 0.1f) ||
            (activateType == 2 && error2 < 0.1f);

        if (!shouldActivate && !alreadyActivated)
            return;

        alreadyActivated = true;

        activationTimer += Time.deltaTime;

        if (activationTimer < triggerDelay)
            return;

        transform.position =
            Vector2.MoveTowards(
                transform.position,
                endPos,
                moveSpeed * Time.deltaTime
            );

        transform.rotation =
            Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0, 0, endRotation),
                moveSpeed * Time.deltaTime
            );

        transform.localScale =
            Vector3.MoveTowards(
                transform.localScale,
                endScale,
                moveSpeed * Time.deltaTime
            );
    }
}
