using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Trigger : MonoBehaviour
{
    TriggerBehavior[] behaviors;

    [Header("Settings")]
    public bool triggerOnce = true;

    bool activated;

    public SpriteRenderer spriteR;

    void Awake()
    {
        behaviors = GetComponents<TriggerBehavior>();
        spriteR = GetComponent<SpriteRenderer>();

        Debug.Log("Found " + behaviors.Length + " trigger behaviors.");
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something entered the trigger: " + other.name);
        if (!other.CompareTag("Player"))
            return;
        
        Debug.Log("Collided with: " + other.name + " Tag: " + other.tag);

        if (triggerOnce && activated)
            return;

        activated = true;

        foreach (TriggerBehavior behavior in behaviors)
        {
            behavior.Execute(other.gameObject);
        }
    }

    public void ResetTrigger()
    {
        activated = false;
    }

    public void UpdateVisuals()
    {
        if(EditorManager.editorOpen)
        {
            spriteR.enabled = true;
        }
        else
        {
            spriteR.enabled = false;
        }
    }
}
