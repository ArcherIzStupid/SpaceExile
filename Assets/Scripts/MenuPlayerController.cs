using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPlayerController : MonoBehaviour
{
    public LevelNode currentNode;

    public float moveSpeed = 5f;

    private LevelNode targetNode;

    public Animator anim;

    private bool moving;
    private int moveDir;
    private float inputDelay = 0.25f;
    private bool canSelectLevel;

    void Start()
    {
        transform.position =
            currentNode.transform.position;
        
        Invoke(nameof(EnableLevelSelect), 0.2f);

        anim = GetComponent<Animator>();

    }
    void EnableLevelSelect()
    {
        canSelectLevel = true;
    }

    void Update()
    {
        inputDelay -= Time.deltaTime;

        if (moving)
        {
            MoveToNode();
            return;
        }

        HandleInput();

        if (inputDelay <= 0 &&
            Keyboard.current.spaceKey.wasPressedThisFrame && canSelectLevel)
        {
            currentNode.LoadLevel();
        }

        HandleAnimations();
    }

    void HandleInput()
    {
        if (inputDelay > 0)
            return;
        
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            TryMove(currentNode.rightNode);
            moveDir = 0;
        }

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            TryMove(currentNode.leftNode);
            moveDir = 2;
        }

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            TryMove(currentNode.upNode);
            moveDir = 1;
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            TryMove(currentNode.downNode);
            moveDir = 3;
        }
    }

    void TryMove(LevelNode node)
    {
        if (node == null)
            return;

        if (!node.unlocked)
            return;

        targetNode = node;

        moving = true;
    }

    void MoveToNode()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetNode.transform.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.05f)
        {
            transform.position =
                targetNode.transform.position;

            currentNode = targetNode;

            moving = false;
        }
    }

    void HandleAnimations()
    {
        if(!moving)
        {
            anim.SetBool("Walk", false);
        }
        else
        {
            anim.SetBool("Walk", true);
        }
        transform.rotation = Quaternion.Euler(0f, 0f, moveDir * 90);
    }
}
