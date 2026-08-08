using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Trigger : MonoBehaviour
{
    private TriggerBehavior[] behaviors;

    [Header("Settings")]
    public bool triggerOnce = true;

    private bool activated;

    private void Awake()
    {
        behaviors =
            GetComponents<TriggerBehavior>();
    }

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Activate(other.gameObject);
    }

    public void Activate(
        GameObject activator)
    {
        if (triggerOnce && activated)
            return;

        activated = true;

        foreach (
            TriggerBehavior behavior
            in behaviors)
        {
            behavior.Execute(activator);
        }
    }

    public void ResetTrigger()
    {
        activated = false;
    }
}