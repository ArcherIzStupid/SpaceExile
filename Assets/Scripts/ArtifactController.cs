using UnityEngine;

public class ArtifactController : MonoBehaviour
{
    public int totalArtifacts;
    public GameObject secretPath;
    public GameObject secretTeleporter;
    void Update()
    {
        UpdateAreaArtifacts();
    }

    void UpdateAreaArtifacts()
    {
        if(totalArtifacts == 21)
        {
            secretPath.SetActive(true);
            secretTeleporter.SetActive(true);
        }
    }

}
