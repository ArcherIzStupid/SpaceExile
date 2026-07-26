using UnityEngine;
using UnityEngine.InputSystem;
using IngameDebugConsole;
using CitrioN.Common;
using System;
using UnityEngine.Animations;

public enum PlayerMode
{
    Normal,
    Jetpack,
    BrokenJetpack,
    InvertingJetpack,
    Gravastar,
    SupermassiveGravastar
}

public enum PlayerSize
{
    Small,
    Normal,
    Big
}
public enum PlayerSpeed
{
    Half,
    Normal,
    Twice,
    Thrice
}
public enum GearType
{
    Jump,
    Gravity,
    DropGravity,
    Drop
}

public enum GravityDirectionType
{
    Down,
    Up,
    Left,
    Right,

    DownRight,
    DownLeft,

    UpRight,
    UpLeft,

    Custom
}
public class PlayerController : MonoBehaviour
{
    //==================================================
    // COMPONENTS
    //==================================================

    [Header("Components")]

    public Rigidbody2D rb;
    public SpriteRenderer spriteR;
    public Animator anim;
    public BoxCollider2D box;
    public CircleCollider2D circle;

    //==================================================
    // MODE/SIZE/SPEED
    //==================================================

    [Header("Mode")]

    public static PlayerMode mode = PlayerMode.Normal;

    [Header("Size")]

    public static PlayerSize size = PlayerSize.Normal;

    public Vector2 smallSize;
    public Vector2 normalSize;
    public Vector2 bigSize;
    
    [Header("Speed")]

    public static PlayerSpeed speed = PlayerSpeed.Normal;

    public float halfSpeedMultiplier = 0.5f;
    public float normalSpeedMultiplier = 1f;
    public float twiceSpeedMultiplier = 2f;
    public float thriceSpeedMultiplier = 3f;

    public float currentSpeed = 1;


    //==================================================
    // MOVEMENT
    //==================================================

    [Header("Movement")]

    public float moveSpeed = 5f;
    public static float jumpForce = 10f;

    [SerializeField] private float acceleration = 2.5f;
    [SerializeField] private float deceleration = 5f;

    public float rotationSpeedG = 500f;

    Quaternion targetRotation;

    //==================================================
    // WALL MOVEMENT
    //==================================================

    [Header("Wall Movement")]

    public float wallSlideSpeed = 5f;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 15f;
    public float wallJumpLockTime = 0.5f;

    //==================================================
    // CHECKS
    //==================================================

    [Header("Checks")]

    public LayerMask ground;

    public float checkRadius = 0.5f;
    public float groundCheckDistance = 1f;

    //==================================================
    // CROUCH
    //==================================================

    [Header("Crouch")]

    public Vector2 playerSize;
    public Vector2 playerOffset;

    public Vector2 crouchSize;
    public Vector2 crouchOffset;

    //==================================================
    // INPUTS
    //==================================================

    [Header("Input")]
    private float moveInput;

    public bool jumpPressed;
    private bool jumpHeld;
    private bool crouching;
    private bool sprinting;

    //==================================================
    // STATES
    //==================================================

    [Header("States")]
    public bool isGrounded;
    public bool isTouchingWall;
    public bool isWallSliding;
    public bool isWallJumping;
    public bool isCrouching;

    //==================================================
    // GRAVASTAR
    //==================================================

    [Header("Gravastar")]
    public float rollTorque = 15f;
    public float maxRollSpeed = 8f;
    public float rotationSpeed = 45;

    //==================================================
    // PRIVATE VARIABLES
    //==================================================

    public static Vector2 gravityDirection =
        Vector2.down;

    public float gravityStrength = 9.81f;

    private float wallDirection;
    private float wallJumpTimer;
    private string currentAnimation;

    //==================================================
    // GEARS
    //==================================================

    [Header("Gear")]

    public static bool canGear;
    //==================================================
    // UNITY METHODS
    //==================================================

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
        circle = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        GetInput();

        CheckGround();

        CheckWall();

        HandleModes();

        HandleWallSlide();

        HandleSize();

        HandleAnimations();

        HandleSpeed();

        HandleGears();
    }

    void FixedUpdate()
    {
        HandleMovement();

        HandleSpriteDirection();

        HandleWallJumpTimer();

        HandleCrouch();

        ApplyGravity();
    }

    void LateUpdate()
    {
        transform.rotation =
            Quaternion.RotateTowards(

                transform.rotation,

                targetRotation,

                rotationSpeed *

                Time.deltaTime
            );
    }

    //==================================================
    // INPUT
    //==================================================

    void GetInput()
    {
        moveInput =
            Keyboard.current.aKey.isPressed ? 1 :
            Keyboard.current.dKey.isPressed ? -1 : 0;

        jumpPressed =
            Keyboard.current.spaceKey.wasPressedThisFrame;
        jumpHeld =
            Keyboard.current.spaceKey.isPressed;

        crouching =
            Keyboard.current.shiftKey.isPressed;

        sprinting =
            Keyboard.current.ctrlKey.isPressed;
    }

    //==================================================
    // CHECKS
    //==================================================

    void CheckGround()
    {
        Vector2 checkPos =

            (Vector2)transform.position

            +

            DownDirection

            * groundCheckDistance;

        isGrounded =

            Physics2D.OverlapCircle(

                checkPos,

                checkRadius,

                ground
            );
    }
    void CheckWall()
    {
        Vector2 leftCheck =

            (Vector2)transform.position

            +

            LeftDirection

            * 0.6f;

        Vector2 rightCheck =

            (Vector2)transform.position

            +

            RightDirection

            * 0.6f;

        bool leftWall =

            Physics2D.OverlapCircle(

                leftCheck,

                checkRadius,

                ground
            );

        bool rightWall =

            Physics2D.OverlapCircle(

                rightCheck,

                checkRadius,

                ground
            );

        isTouchingWall =

            leftWall || rightWall;

        if(leftWall)
        {
            wallDirection = 1;
        }

        else if(rightWall)
        {
            wallDirection = -1;
        }

        else
        {
            wallDirection = 0;
        }
    }

    //==================================================
    // MODES
    //==================================================

    void HandleModes()
    {

        switch (mode)
        {
            case PlayerMode.Normal:
                HandleNormalMode();
                break;

            case PlayerMode.Jetpack:
                HandleJetpackMode();
                break;

            case PlayerMode.BrokenJetpack:
                HandleBrokenJetpackMode();
                break;

            case PlayerMode.InvertingJetpack:
                HandleInvertingJetpackMode();
                break;

            case PlayerMode.Gravastar:
                HandleGravastarMode();
                break;

            case PlayerMode.SupermassiveGravastar:
                HandleSupermassiveGravastarMode();
                break;
        }
    }

    void HandleNormalMode()
    {
        box.enabled = true;
        circle.enabled = false;
        if(jumpPressed)
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (isTouchingWall)
            {
                WallJump();
            }
        }
    }

    void HandleJetpackMode()
    {
        box.enabled = true;
        circle.enabled = false;
        if (!jumpHeld)
            return;

        rb.AddForce(
            UpDirection *
            jumpForce *
            0.5f,

            ForceMode2D.Force
        );

        // Optional max vertical speed
        float sideSpeed =

            Vector2.Dot(
            
                rb.linearVelocity,

                RightDirection
            );

        float gravitySpeed =

            Mathf.Clamp(
            
                Vector2.Dot(
                
                    rb.linearVelocity,

                    UpDirection
                ),

                -8f,

                8f
            );

        rb.linearVelocity =

            RightDirection

            * sideSpeed

            +

            UpDirection

            * gravitySpeed;
    }

    void HandleBrokenJetpackMode()
    {
        box.enabled = true;
        circle.enabled = false;
        if(jumpPressed)
        {
            Jump();
        }
    }

    void HandleInvertingJetpackMode()
    {
        box.enabled = true;
        circle.enabled = false;
        // Continuous flying
        if (jumpHeld)
        {
            rb.AddForce(
                UpDirection *
                jumpForce,

                ForceMode2D.Force
            );

            float sideSpeed =

                Vector2.Dot(
                
                    rb.linearVelocity,

                    RightDirection
                );

            float gravitySpeed =

                Mathf.Clamp(
                
                    Vector2.Dot(
                    
                        rb.linearVelocity,

                        UpDirection
                    ),

                    -8f,

                    8f
                );

            rb.linearVelocity =

                RightDirection

                * sideSpeed

                +

                UpDirection

                * gravitySpeed;
                
        }

        // Flip gravity ONLY once per press
        if (jumpPressed)
        {
            FlipGravity();
        }
    }

    void HandleGravastarMode()
    {
        circle.enabled = true;
        box.enabled = false;
        if (isGrounded && jumpPressed)
        {
            FlipGravity();

            Jump();
        }
        HandleGravastarRotation();
    }

    void HandleSupermassiveGravastarMode()
    {
        circle.enabled = true;
        box.enabled = false;
        
        if (isGrounded && jumpPressed)
        {
            TpToPlatform();
            FlipGravity();
        }
        HandleGravastarRotation();
    }

    //==================================================
    // SIZE
    //==================================================

    void HandleSize()
    {
        switch(size)
        {
            case PlayerSize.Small:
                HandleSmallSize();
                return;
            case PlayerSize.Normal:
                HandleNormalSize();
                return;
            case PlayerSize.Big:
                HandleBigSize();
                return;
        }
    }

    void HandleSmallSize()
    {
        transform.localScale = smallSize;
        jumpForce = 13;
    }

    void HandleNormalSize()
    {
        transform.localScale = normalSize;
        jumpForce = 10;
    }

    void HandleBigSize()
    {
        transform.localScale = bigSize;
        jumpForce = 7;
    }

    //==================================================
    // SPEED
    //==================================================

    void HandleSpeed()
    {
        switch(speed)
        {
            case PlayerSpeed.Half:
                currentSpeed = 0.5f;
                return;
            case PlayerSpeed.Normal:
                currentSpeed = 1f;
                return;
            case PlayerSpeed.Twice:
                currentSpeed = 2f;
                return;
            case PlayerSpeed.Thrice:
                currentSpeed = 3f;
                return;
        }
    }


    //==================================================
    // MOVEMENT
    //==================================================

    void HandleMovement()
    {
        if (mode == PlayerMode.Gravastar ||
        mode == PlayerMode.SupermassiveGravastar)
        {
            HandleGravastarMovement();
            return;
        }

        if (isWallJumping || crouching)
            return;
        float targetSpeed =
            moveInput *
            moveSpeed *
            currentSpeed;

        float currentVelocity =
            Vector2.Dot(
                rb.linearVelocity,

                RightDirection
            );
        float speedDif =
            targetSpeed -
            currentVelocity;

        float accelRate =
            Mathf.Abs(targetSpeed) > 0.01f
            ? acceleration
            : deceleration;

        float moveForce =
            speedDif * accelRate;

        if(
            mode == PlayerMode.Normal
        
            ||
        
            isGrounded
        )
        {
            rb.AddForce(
            
                moveForce
        
                * RightDirection,
        
                ForceMode2D.Force
            );
        }

        float horizontal =
            Mathf.Clamp(
            
                Vector2.Dot(
                    rb.linearVelocity,

                    RightDirection
                ),

                -moveSpeed,

                moveSpeed
            );

        float vertical =
            Vector2.Dot(
                rb.linearVelocity,

                UpDirection
            );

        rb.linearVelocity =

            RightDirection

            * horizontal

            +

            UpDirection

            * vertical;
    }

    void HandleGravastarMovement()
    {
        if (!isGrounded)
            return;

        rb.AddTorque(
            -moveInput *
            rollTorque,
            ForceMode2D.Force
        );

        rb.linearVelocity = new Vector2(
            Mathf.Clamp(
                Vector2.Dot(

                    rb.linearVelocity,

                    RightDirection
                ),
                -maxRollSpeed,
                maxRollSpeed
            ),
            rb.linearVelocity.y
        );
    }

    void HandleGravastarRotation()
    {
        if (mode != PlayerMode.Gravastar &&
            mode != PlayerMode.SupermassiveGravastar)
            return;
    
        float rotationAmount =
            Vector2.Dot(

                rb.linearVelocity,

                RightDirection
            ) *
            rotationSpeed *
            Time.fixedDeltaTime;
    
        transform.Rotate(
            0f,
            0f,
            -rotationAmount
        );
    }

    void Jump()
    {
        anim.SetTrigger("Jump");

        Vector2 sideways =
            Project(
                rb.linearVelocity,

                RightDirection
            );

        rb.linearVelocity =

            sideways

            +

            UpDirection

            * jumpForce;
    }

    void WallJump()
    {
        isWallJumping = true;

        isWallSliding = false;

        wallJumpTimer = wallJumpLockTime;

        // Clear existing velocity
        rb.linearVelocity = Vector2.zero;

        Vector2 jumpDirection =
            (wallDirection * RightDirection)
            +
            UpDirection;

        jumpDirection.Normalize();

        rb.AddForce(
            jumpDirection *
            wallJumpForceX,

            ForceMode2D.Impulse
        );
    }

    void HandleWallSlide()
    {
        if (!isTouchingWall ||
            isGrounded ||
            isWallJumping)
        {
            isWallSliding = false;
    
            return;
        }
    
        isWallSliding = true;
    
        Vector2 horizontal =
            Project(
                rb.linearVelocity,
    
                RightDirection
            );
    
        float gravitySpeed =
            Mathf.Min(
            
                Vector2.Dot(
                    rb.linearVelocity,
    
                    gravityDirection
                ),
    
                wallSlideSpeed
            );
    
        rb.linearVelocity =
    
            horizontal
    
            +
    
            gravityDirection
    
            * gravitySpeed;
    }

    //==================================================
    // GRAVITY
    //==================================================

    public Vector2 UpDirection
    {
        get
        {
            return -gravityDirection;
        }
    }

    public Vector2 DownDirection
    {
        get
        {
            return gravityDirection;
        }
    }

    public Vector2 LeftDirection
    {
        get
        {
            return -RightDirection;
        }
    }

    public Vector2 RightDirection
    {
        get
        {
            return new Vector2(
                gravityDirection.y,

                -gravityDirection.x
            );
        }
    }

    Vector2 Project(Vector2 vector, Vector2 onto)
    {
        if (onto.sqrMagnitude < 0.0001f)
            return Vector2.zero;

        return onto *
               (
                   Vector2.Dot(
                       vector,
                       onto
                   )
                   /
                   onto.sqrMagnitude
               );
    }

    void FlipGravity()
    {
        gravityDirection *= -1;

        UpdatePlayerRotation();
    }

    void ApplyGravity()
    {
        rb.gravityScale = 0;

        rb.AddForce(
            gravityDirection *

            gravityStrength,

            ForceMode2D.Force
        );
    }

    void UpdatePlayerRotation()
    {
        rb.freezeRotation = false;
        float angle =
            Mathf.Atan2(
                gravityDirection.y,

                gravityDirection.x
            )

            * Mathf.Rad2Deg

            + 90f;

        transform.rotation =
            Quaternion.Euler(
                0,

                0,

                angle
            );
        rb.freezeRotation = true;
    }

    public void SetGravity(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
        {
            Debug.LogWarning(
                "Invalid gravity direction."
            );

            return;
        }

        gravityDirection =
            direction.normalized;

        UpdatePlayerRotation();
        
        Debug.Log("Gravity: " + gravityDirection);
    }

    public Vector2 RotateVector(
        Vector2 input
    )
    {
        float angle =

            Mathf.Atan2(

                gravityDirection.y,

                gravityDirection.x

            )

            - Mathf.PI / 2f;

        return new Vector2(

            input.x * Mathf.Cos(angle)

            -

            input.y * Mathf.Sin(angle),

            input.x * Mathf.Sin(angle)

            +

            input.y * Mathf.Cos(angle)

        );
    }

    //==================================================
    // CROUCH
    //==================================================

    void HandleCrouch()
    {
        if (mode != PlayerMode.Normal)
            return;

        if (crouching)
        {
            Crouch();
        }
        else if (CanStand())
        {
            Stand();
        }
    }

    void Crouch()
    {
        if (isCrouching)
            return;

        isCrouching = true;

        box.size = crouchSize;

        box.offset = crouchOffset;
    }

    void Stand()
    {
        if (!isCrouching)
            return;

        isCrouching = false;

        box.size = playerSize;

        box.offset = playerOffset;
    }

    bool CanStand()
    {
        return !Physics2D.Raycast(
            transform.position,

            UpDirection,

            1f,

            ground
        );
    }

    //==================================================
    // VISUALS
    //==================================================

    void HandleSpriteDirection()
    {
        if (isWallJumping)
            return;

        if (moveInput > 0)
        {
            spriteR.flipX = true;
        }
        else if (moveInput < 0)
        {
            spriteR.flipX = false;
        }
    }

    void HandleAnimations()
    {
        anim.SetBool("Slide", isWallSliding);

        anim.SetBool("WJump", isWallJumping);

        anim.SetBool("Crouch", crouching);

        anim.SetBool("isGrounded", isGrounded);

        anim.SetFloat("Speed", Mathf.Abs(Vector2.Dot(rb.linearVelocity, RightDirection)));

        anim.SetFloat("WJTimer", wallJumpTimer);

        anim.SetInteger("Mode", mode == PlayerMode.Normal ? 0 : mode == PlayerMode.Jetpack ? 1 : mode == PlayerMode.BrokenJetpack ? 2 : mode == PlayerMode.InvertingJetpack ? 3 : mode == PlayerMode.Gravastar ? 4 : mode == PlayerMode.SupermassiveGravastar ? 5 : 6);

        anim.SetBool("isFlying", jumpHeld && mode != PlayerMode.Normal);
        
        anim.SetFloat("YSpeed", Vector2.Dot(rb.linearVelocity, UpDirection));

        
        switch(mode)
        {
            case PlayerMode.Normal:
                PlayAnimation("Idle");
                break;
            case PlayerMode.Jetpack:
                PlayAnimation("JetpackIdle");
                break;
            case PlayerMode.BrokenJetpack:
                PlayAnimation("BrokenJetpackIdle");
                break;
            case PlayerMode.InvertingJetpack:
                PlayAnimation("InvertingJetpackIdle");
                break;
            case PlayerMode.Gravastar:
                PlayAnimation("Gravastar");
                break;
            case PlayerMode.SupermassiveGravastar:
                PlayAnimation("SupermassiveGravastar");
                break;
        }
    }

    void PlayAnimation(string animationName)
    {
        if (currentAnimation == animationName)
            return;

        currentAnimation = animationName;

        anim.Play(animationName);
    }

    //==================================================
    // TIMERS
    //==================================================

    void HandleWallJumpTimer()
    {
        wallJumpTimer -= Time.deltaTime;

        if (wallJumpTimer <= 0)
        {
            isWallJumping = false;
        }
    }

    //==================================================
    // TELEPORT
    //==================================================

    void TpToPlatform()
    {
        RaycastHit2D hit =
            Physics2D.Raycast(

                transform.position,

                UpDirection,

                20f,

                ground
            );

        if(hit.collider == null)
            return;

        transform.position =

            hit.point

            -

            UpDirection;
    }

    //==================================================
    // RESPAWN
    //==================================================

    public void Respawn()
    {
        transform.position = new Vector2(-10f, 3.4f);
    
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    
        transform.rotation = Quaternion.identity;
    
        isGrounded = true;
        isWallSliding = false;
        isWallJumping = false;
        isTouchingWall = false;
    
        spriteR.flipX = false;
        gravityDirection = Vector2.down;

        UpdatePlayerRotation();
    
        currentAnimation = "";
    }

    //==================================================
    // COLLISIONS
    //==================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Level_Exit"))
        {
            Debug.Log("Exiting...");
        }

        if (other.CompareTag("^"))
        {
            SetGravity(Vector2.up);
        }

        if (other.CompareTag("Down"))
        {
            SetGravity(Vector2.down);
        }
    }

    //==================================================
    // UI
    //==================================================

    public void OnPause()
    {
        Time.timeScale = 0f;
    }

    public void OnResume()
    {
        Time.timeScale = 1f;
    }

    //==================================================
    // GEARS
    //==================================================

    void HandleGears()
    {
        if(!jumpPressed)
            return;

        if(!canGear)
            return;

        switch(GearController.currentGear.gearType)
        {
            case GearType.Jump:

                rb.linearVelocity +=

                    UpDirection

                    *

                    jumpForce

                    *

                    GearController.currentGear
                    .gearForce;

                break;

            case GearType.Gravity:

                SetGravity(
                    GearController.currentGear
                    .EndGravityVector
                );

                break;

            case GearType.DropGravity:

                SetGravity(
                    GearController.currentGear
                    .EndGravityVector
                );

                rb.linearVelocity =

                    gravityDirection

                    * 10f;

                break;

            case GearType.Drop:

                rb.linearVelocity =

                    gravityDirection

                    * 10f;

                break;
        }
    }
}
