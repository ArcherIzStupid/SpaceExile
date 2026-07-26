using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelNode : MonoBehaviour
{
    public string sceneName;

    public bool unlocked;
    public bool completed;

    public SpriteRenderer spriteR;
    void Start()
    {
        spriteR = spriteR.GetComponent<SpriteRenderer>();
    }

    [Header("Connections")]
    public LevelNode upNode;
    public LevelNode downNode;
    public LevelNode leftNode;
    public LevelNode rightNode;
    public Color lockedColor = Color.gray;
    public Color unlockedColor = Color.white;
    public Color completedColor = Color.green; 

    void Update()
    {
        LoadLevel();
        ChangeColor();
    }
    public void LoadLevel()
    {
        if (!unlocked)
            return;
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            SceneManager.LoadScene(sceneName);
    }

    void ChangeColor()
    {
        if(!unlocked) spriteR.color = lockedColor;
        if(unlocked && !completed) spriteR.color = unlockedColor;
        if(completed) spriteR.color = completedColor;
    }
}