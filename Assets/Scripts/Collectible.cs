using UnityEngine;

public class Collectible : MonoBehaviour
{
    public ArtifactController artifact;

    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (artifact == null)
            artifact = FindFirstObjectByType<ArtifactController>();

        if (artifact != null)
            artifact.totalArtifacts += amount;

        Destroy(gameObject);
    }
}
