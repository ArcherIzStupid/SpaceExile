using UnityEngine;
using UnityEngine.InputSystem;
using IngameDebugConsole;
using CitrioN.Common;
using System;
using UnityEngine.Animations;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

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
    [SerializeField] public float jumpForce = 10f;

    [SerializeField] private float acceleration = 2.5f;
    [SerializeField] private float deceleration = 5f;

    public float rotationSpeedG = 500f;

    Quaternion targetRotation;

    [Header("Jetpack")]
    [SerializeField] private float jetpackForce = 30f;
    [SerializeField] private float jetpackMaximumSpeed = 14f;

    [Header("Gravity Rotation")]
    [SerializeField] private float gravityRotationSpeed = 500f;
    [SerializeField] private bool rotatePlayerWithGravity = true;

    private float targetGravityAngle;

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
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private float wallCheckDistance = 0.65f;

    [Header("Control Direction")]
    [SerializeField] private bool invertControlsOnStart = false;

    [Tooltip("Manually reverses the final A/D controls.")]
    [SerializeField] public bool invertControls;

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
    [SerializeField] private float gravastarMoveForce = 25f;

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
    // ANIMATIONS
    //==================================================

    [SerializeField] private int NormalModeLayer;
    [SerializeField] private int JetpackModeLayer;
    [SerializeField] private int BrokenModeLayer;
    [SerializeField] private int InvertingModeLayer;
    [SerializeField] private int GravastarModeLayer;
    [SerializeField] private int SupermassiveGravastarModeLayer;

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

        gravityDirection =
            gravityDirection.normalized;

        UpdatePlayerRotation();

        rb.rotation = targetGravityAngle;
        rb.freezeRotation = false;

        invertControls = invertControlsOnStart;

        NormalModeLayer = anim.GetLayerIndex("Normal Mode");
        JetpackModeLayer = anim.GetLayerIndex("Jetpack Mode");
        BrokenModeLayer = anim.GetLayerIndex("Broken Mode");
        InvertingModeLayer = anim.GetLayerIndex("Inverting Mode");
        GravastarModeLayer = anim.GetLayerIndex("Gravastar Mode");
        SupermassiveGravastarModeLayer = anim.GetLayerIndex("Supermassive Gravastar Mode");
    }

    void Update()
    {
        GetInput();

        HandleModes();
        HandleSize();
        HandleSpeed();
        HandleGears();
        HandleAnimations();
        HandleModeAnimations();
    }

    void FixedUpdate()
    {
        CheckGround();
        CheckWall();

        HandleMovement();
        HandleWallSlide();
        HandleJetpackPhysics();

        HandleSpriteDirection();
        HandleWallJumpTimer();
        HandleCrouch();

        ApplyGravity();

        HandleGravityRotation();
    }

    //==================================================
    // INPUT
    //==================================================

    void GetInput()
    {
            float rawMoveInput = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            rawMoveInput = -1f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            rawMoveInput = 1f;
        }

        bool finalInversion =
            IsGravityInInvertedControlRange() ^ invertControls;

        moveInput =
            finalInversion
                ? -rawMoveInput
                : rawMoveInput;

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
        Vector2 playerPosition = transform.position;
    
        Vector2 leftCheck =
            playerPosition +
            LeftDirection * wallCheckDistance;
    
        Vector2 rightCheck =
            playerPosition +
            RightDirection * wallCheckDistance;
    
        bool leftWall =
            Physics2D.OverlapCircle(
                leftCheck,
                checkRadius,
                ground);
    
        bool rightWall =
            Physics2D.OverlapCircle(
                rightCheck,
                checkRadius,
                ground);
    
        isTouchingWall = leftWall || rightWall;
    
        if (leftWall)
        {
            // Jump toward the right.
            wallDirection = 1f;
        }
        else if (rightWall)
        {
            // Jump toward the left.
            wallDirection = -1f;
        }
        else
        {
            wallDirection = 0f;
        }
    }
    bool IsGravityInInvertedControlRange()
    {
        float gravityAngle = Mathf.Atan2(gravityDirection.y, gravityDirection.x) * Mathf.Rad2Deg + 90;
    
        // Measures the shortest distance to 0 degrees. 
        // Returns a value between -180 and 180.
        //float relativeAngle = Mathf.DeltaAngle(0f, gravityAngle);
    
        // Checks if the angle is within 91 degrees of 0 in either direction
        return Mathf.Abs(gravityAngle) >= 91f;
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

        if (jumpPressed)
        {
            FlipGravity();

            // Prevent the old velocity from fighting the newly
            // inverted jetpack direction.
            float sidewaysSpeed =
                Vector2.Dot(
                    rb.linearVelocity,
                    RightDirection);

            rb.linearVelocity =
                RightDirection * sidewaysSpeed;
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

        bool isJetpackMode =
            mode == PlayerMode.Jetpack ||
            mode == PlayerMode.BrokenJetpack ||
            mode == PlayerMode.InvertingJetpack;

        // Jetpack types steer only while airborne.
        // Normal mode can move both on the ground and in the air.
        bool canMove =
            !isJetpackMode || !isGrounded;

        if (!canMove)
            return;

        float targetSpeed =
            moveInput *
            moveSpeed *
            currentSpeed;

        float currentSidewaysSpeed =
            Vector2.Dot(
                rb.linearVelocity,
                RightDirection);

        float speedDifference =
            targetSpeed -
            currentSidewaysSpeed;

        float accelerationRate =
            Mathf.Abs(targetSpeed) > 0.01f
                ? acceleration
                : deceleration;

        rb.AddForce(
            RightDirection *
            speedDifference *
            accelerationRate,
            ForceMode2D.Force);

        float maximumSpeed =
            moveSpeed *
            currentSpeed;

        float clampedSidewaysSpeed =
            Mathf.Clamp(
                Vector2.Dot(
                    rb.linearVelocity,
                    RightDirection),
                -maximumSpeed,
                maximumSpeed);

        float gravityAxisSpeed =
            Vector2.Dot(
                rb.linearVelocity,
                UpDirection);

        rb.linearVelocity =
            RightDirection * clampedSidewaysSpeed +
            UpDirection * gravityAxisSpeed;
    }

    void HandleJetpackPhysics()
    {
        bool usesContinuousJetpack =
            mode == PlayerMode.Jetpack ||
            mode == PlayerMode.InvertingJetpack;

        if (!usesContinuousJetpack || !jumpHeld)
            return;

        rb.AddForce(
            UpDirection * jetpackForce,
            ForceMode2D.Force);

        float sidewaysSpeed =
            Vector2.Dot(
                rb.linearVelocity,
                RightDirection);

        float upwardSpeed =
            Vector2.Dot(
                rb.linearVelocity,
                UpDirection);

        upwardSpeed =
            Mathf.Clamp(
                upwardSpeed,
                -jetpackMaximumSpeed,
                jetpackMaximumSpeed);

        rb.linearVelocity =
            RightDirection * sidewaysSpeed +
            UpDirection * upwardSpeed;
    }

    void HandleGravastarMovement()
    {
        if (!isGrounded)
            return;

        // Torque provides the rolling visual and physical rotation.
        rb.AddTorque(
            -moveInput * rollTorque,
            ForceMode2D.Force);

        // Side force guarantees that the ball actually translates,
        // even when contact friction is weak.
        rb.AddForce(
            RightDirection *
            moveInput *
            gravastarMoveForce,
            ForceMode2D.Force);

        float sidewaysSpeed =
            Vector2.Dot(
                rb.linearVelocity,
                RightDirection);

        sidewaysSpeed =
            Mathf.Clamp(
                sidewaysSpeed,
                -maxRollSpeed,
                maxRollSpeed);

        float gravityAxisSpeed =
            Vector2.Dot(
                rb.linearVelocity,
                UpDirection);

        rb.linearVelocity =
            RightDirection * sidewaysSpeed +
            UpDirection * gravityAxisSpeed;
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
            rotationAmount
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
        bool modeAllowsWallSliding =
            mode == PlayerMode.Normal ||
            mode == PlayerMode.Jetpack ||
            mode == PlayerMode.BrokenJetpack ||
            mode == PlayerMode.InvertingJetpack;

        if (!modeAllowsWallSliding ||
            !isTouchingWall ||
            isGrounded ||
            isWallJumping)
        {
            isWallSliding = false;
            return;
        }

        isWallSliding = true;

        float sidewaysSpeed =
            Vector2.Dot(
                rb.linearVelocity,
                RightDirection);

        float fallingSpeed =
            Vector2.Dot(
                rb.linearVelocity,
                DownDirection);

        // Positive means moving with gravity.
        fallingSpeed =
            Mathf.Min(
                fallingSpeed,
                wallSlideSpeed);

        rb.linearVelocity =
            RightDirection * sidewaysSpeed +
            DownDirection * fallingSpeed;
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
                -gravityDirection.y,
                gravityDirection.x);
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
        if (!IsFiniteVector(gravityDirection) ||
            gravityDirection.sqrMagnitude < 0.0001f)
        {
            Debug.LogWarning(
                $"Invalid gravity direction: {gravityDirection}. " +
                "Resetting gravity to down.");

            gravityDirection = Vector2.down;
        }

        gravityDirection.Normalize();

        // Down  =   0°
        // Right =  90°
        // Up    = 180°
        // Left  = -90°
        targetGravityAngle =
            Mathf.Atan2(
                gravityDirection.x,
                -gravityDirection.y)
            * Mathf.Rad2Deg;

        if (!float.IsFinite(targetGravityAngle))
        {
            Debug.LogError(
                "Gravity rotation angle became invalid. " +
                "Resetting to 0.");

            targetGravityAngle = 0f;
            gravityDirection = Vector2.down;
        }
    }

    public void SetGravity(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
        {
            Debug.LogWarning(
                "SetGravity received a zero direction.");

            return;
        }

        gravityDirection =
            direction.normalized;

        UpdatePlayerRotation();

        Debug.Log(
            $"Gravity changed to {gravityDirection}. " +
            $"Target angle: {targetGravityAngle}");
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

    void HandleGravityRotation()
    {
        if (!rotatePlayerWithGravity)
            return;

        if (mode == PlayerMode.Gravastar ||
            mode == PlayerMode.SupermassiveGravastar)
        {
            return;
        }

        if (!float.IsFinite(targetGravityAngle))
        {
            Debug.LogError(
                "Invalid target gravity angle detected.");

            targetGravityAngle = 0f;
            gravityDirection = Vector2.down;
        }

        rb.angularVelocity = 0f;

        float nextAngle =
            Mathf.MoveTowardsAngle(
                rb.rotation,
                targetGravityAngle,
                gravityRotationSpeed *
                Time.fixedDeltaTime);

        rb.MoveRotation(nextAngle);
    }

    public bool IsFiniteVector(Vector2 v)
    {
        if(float.IsNaN(v.x) || float.IsNaN(v.y))
        {
            return false;
        }
        else if(float.IsInfinity(v.x) || float.IsInfinity(v.y))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public float Vector2Angle(Vector2 input)
    {
        return Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg + 90;
    }

    public Vector2 Angle2Vector(float input)
    {
        Vector2 gravityDir = new Vector2(Mathf.Cos((input - 90f) * Mathf.Deg2Rad), Mathf.Sin((input - 90f) * Mathf.Deg2Rad));
        return gravityDir;
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

        if (moveInput < 0f)
        {
            spriteR.flipX = !invertControls;
        }
        else if (moveInput > 0f)
        {
            spriteR.flipX = invertControls;
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

        anim.SetBool("isFlying", jumpHeld && mode == PlayerMode.Jetpack || mode == PlayerMode.InvertingJetpack);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("YSpeed", Vector2.Dot(rb.linearVelocity, UpDirection));
        anim.SetInteger("Mode", (int)mode);
    }

    void HandleModeAnimations()
    {
        if(mode == PlayerMode.Normal)
        {
            anim.SetLayerWeight(NormalModeLayer, Mathf.Lerp(anim.GetLayerWeight(NormalModeLayer), 1f, Time.deltaTime * 10f));
            anim.SetLayerWeight(JetpackModeLayer, Mathf.Lerp(anim.GetLayerWeight(JetpackModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(BrokenModeLayer, Mathf.Lerp(anim.GetLayerWeight(BrokenModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(InvertingModeLayer, Mathf.Lerp(anim.GetLayerWeight(InvertingModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(GravastarModeLayer, Mathf.Lerp(anim.GetLayerWeight(GravastarModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(SupermassiveGravastarModeLayer, Mathf.Lerp(anim.GetLayerWeight(SupermassiveGravastarModeLayer), 0f, Time.deltaTime * 10f));
        }
        if(mode == PlayerMode.Jetpack)
        {
            anim.SetLayerWeight(NormalModeLayer, Mathf.Lerp(anim.GetLayerWeight(NormalModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(JetpackModeLayer, Mathf.Lerp(anim.GetLayerWeight(JetpackModeLayer), 1f, Time.deltaTime * 10f));
            anim.SetLayerWeight(BrokenModeLayer, Mathf.Lerp(anim.GetLayerWeight(BrokenModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(InvertingModeLayer, Mathf.Lerp(anim.GetLayerWeight(InvertingModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(GravastarModeLayer, Mathf.Lerp(anim.GetLayerWeight(GravastarModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(SupermassiveGravastarModeLayer, Mathf.Lerp(anim.GetLayerWeight(SupermassiveGravastarModeLayer), 0f, Time.deltaTime * 10f));
        }
        if(mode == PlayerMode.BrokenJetpack)
        {
            anim.SetLayerWeight(NormalModeLayer, Mathf.Lerp(anim.GetLayerWeight(NormalModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(JetpackModeLayer, Mathf.Lerp(anim.GetLayerWeight(JetpackModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(BrokenModeLayer, Mathf.Lerp(anim.GetLayerWeight(BrokenModeLayer), 1f, Time.deltaTime * 10f));
            anim.SetLayerWeight(InvertingModeLayer, Mathf.Lerp(anim.GetLayerWeight(InvertingModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(GravastarModeLayer, Mathf.Lerp(anim.GetLayerWeight(GravastarModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(SupermassiveGravastarModeLayer, Mathf.Lerp(anim.GetLayerWeight(SupermassiveGravastarModeLayer), 0f, Time.deltaTime * 10f));
        }
        if(mode == PlayerMode.InvertingJetpack)
        {
            anim.SetLayerWeight(NormalModeLayer, Mathf.Lerp(anim.GetLayerWeight(NormalModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(JetpackModeLayer, Mathf.Lerp(anim.GetLayerWeight(JetpackModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(BrokenModeLayer, Mathf.Lerp(anim.GetLayerWeight(BrokenModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(InvertingModeLayer, Mathf.Lerp(anim.GetLayerWeight(InvertingModeLayer), 1f, Time.deltaTime * 10f));
            anim.SetLayerWeight(GravastarModeLayer, Mathf.Lerp(anim.GetLayerWeight(GravastarModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(SupermassiveGravastarModeLayer, Mathf.Lerp(anim.GetLayerWeight(SupermassiveGravastarModeLayer), 0f, Time.deltaTime * 10f));
        }
        if(mode == PlayerMode.Gravastar)
        {
            anim.SetLayerWeight(NormalModeLayer, Mathf.Lerp(anim.GetLayerWeight(NormalModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(JetpackModeLayer, Mathf.Lerp(anim.GetLayerWeight(JetpackModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(BrokenModeLayer, Mathf.Lerp(anim.GetLayerWeight(BrokenModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(InvertingModeLayer, Mathf.Lerp(anim.GetLayerWeight(InvertingModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(GravastarModeLayer, Mathf.Lerp(anim.GetLayerWeight(GravastarModeLayer), 1f, Time.deltaTime * 10f));
            anim.SetLayerWeight(SupermassiveGravastarModeLayer, Mathf.Lerp(anim.GetLayerWeight(SupermassiveGravastarModeLayer), 0f, Time.deltaTime * 10f));
        }
        if(mode == PlayerMode.SupermassiveGravastar)
        {
            anim.SetLayerWeight(NormalModeLayer, Mathf.Lerp(anim.GetLayerWeight(NormalModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(JetpackModeLayer, Mathf.Lerp(anim.GetLayerWeight(JetpackModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(BrokenModeLayer, Mathf.Lerp(anim.GetLayerWeight(BrokenModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(InvertingModeLayer, Mathf.Lerp(anim.GetLayerWeight(InvertingModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(GravastarModeLayer, Mathf.Lerp(anim.GetLayerWeight(GravastarModeLayer), 0f, Time.deltaTime * 10f));
            anim.SetLayerWeight(SupermassiveGravastarModeLayer, Mathf.Lerp(anim.GetLayerWeight(SupermassiveGravastarModeLayer), 1f, Time.deltaTime * 10f));
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
                ground);

        if (hit.collider == null)
            return;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position =
            hit.point -
            UpDirection;

        Physics2D.SyncTransforms();
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
