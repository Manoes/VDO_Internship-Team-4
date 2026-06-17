using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MonkeyPlayerController : MonoBehaviour
{
    [SerializeField] private int playerIndex = 1;

    [Header("Movement Parameters")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float groundAcceleration = 70f;
    [SerializeField] private float groundDeceleration = 90f;
    [SerializeField] private float airAcceleration = 45f;
    [SerializeField] private float airDeceleration = 35f;

    [Header("Safety Clamp")]
    [SerializeField] private float maxHorizontalVelocity = 12f;
    [SerializeField] private float maxVerticalVelocity = 25f;

    [Header("Jump")]
    [SerializeField] private float jumpVelocity = 14f;
    [SerializeField] private float coyoteTime = 0.10f;
    [SerializeField] private float jumpBufferTime = 0.10f;

    [Header("Gravity Feel")]
    [SerializeField] private float fallGravityMultiplier = 2.0f;
    [SerializeField] private float maxFallSpeed = 22f;

    [Header("Wall Jump")]
    [SerializeField] private bool enableWallJump = true;
    [SerializeField] private float wallSlideMaxFallSpeed = 4.5f;
    [SerializeField] private float wallJumpXVelocity = 9f;
    [SerializeField] private float wallJumpYVelocity = 14f;
    [SerializeField] private float wallJumpLockTime = 0.12f;

    [Header("Collision Checks")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.65f, 0.10f);
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.10f, 0.9f);
    [SerializeField] private float checkDistance = 0.05f;

    [Header("Anti Double-Jump")]
    [SerializeField] private float ignoreGroundedAfterJump = 0.08f;

    [Header("Visuals")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Visual Juice")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float idleBreathSpeed = 4f;
    [SerializeField] private float idleBreathAmount = 0.04f;
    [SerializeField] private float squashReturnSpeed = 12f;

    Rigidbody2D rb;
    Collider2D col;

    float inputX;

    private Vector2 moveInput;

    float coyoteTimer;
    float jumpBufferTimer;
    bool isJumping;

    bool isGrounded;
    private bool wasGrounded;
    bool rawGrounded;
    bool onWallLeft;
    bool onWallRight;

    float wallJumpLockTimer;
    int wallDirection;

    float ignoreGroundedTimer;

    public int PlayerIndex => playerIndex;
    public bool IsGrounded => isGrounded;
    public bool IsOnWall => !isGrounded && wallDirection != 0;
    public int LastJumpFrame { get; private set; } = -9999;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        if (visualRoot == null && spriteRenderer != null)
            visualRoot = spriteRenderer.transform;
    }

    void Update()
    {
        inputX = moveInput.x;

        jumpBufferTimer -= Time.deltaTime;

        if (ignoreGroundedTimer > 0f)
            ignoreGroundedTimer -= Time.deltaTime;

        UpdateCollisionStates();

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        UpdateAnimations();
    }

    void FixedUpdate()
    {
        UpdateCollisionStates();

        MoveHorizontally(inputX);

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            DoJump(jumpVelocity);

            isJumping = true;

            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            ignoreGroundedTimer = ignoreGroundedAfterJump;
            LastJumpFrame = Time.frameCount;
        }
        else if (jumpBufferTimer > 0f && enableWallJump && !isGrounded && wallDirection != 0)
        {
            float launchX = -wallDirection * wallJumpXVelocity;
            rb.linearVelocity = new Vector2(launchX, wallJumpYVelocity);

            isJumping = true;

            wallJumpLockTimer = wallJumpLockTime;
            jumpBufferTimer = 0f;
            ignoreGroundedTimer = ignoreGroundedAfterJump;
            LastJumpFrame = Time.frameCount;
        }

        if (enableWallJump && !isGrounded && wallDirection != 0 && rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                Mathf.Max(rb.linearVelocity.y, -wallSlideMaxFallSpeed)
            );
        }

        ApplyGravityFeel();

        HandleScreenWrap();

        if (wallJumpLockTimer > 0f)
            wallJumpLockTimer -= Time.fixedDeltaTime;

        ClampVelocity();
    }

    private void HandleScreenWrap()
    {
        if (ScreenWrapManager.Instance == null) return;

        Vector3 wrappedPosition =
            ScreenWrapManager.Instance.WrapPosition(rb.position);

        rb.position = wrappedPosition;
    }

    private void ClampVelocity()
    {
        rb.linearVelocity = new Vector2(
            Mathf.Clamp(rb.linearVelocity.x, -maxHorizontalVelocity, maxHorizontalVelocity),
            Mathf.Clamp(rb.linearVelocity.y, -maxVerticalVelocity, maxVerticalVelocity)
        );
    }

    void MoveHorizontally(float inputX)
    {
        if (wallJumpLockTimer > 0f)
            inputX = 0f;

        float targetSpeed = inputX * maxSpeed;

        float accelRate = Mathf.Abs(inputX) > 0.01f
            ? (isGrounded ? groundAcceleration : airAcceleration)
            : (isGrounded ? groundDeceleration : airDeceleration);

        float newX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            accelRate * Time.deltaTime
        );

        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
    }

    void DoJump(float velY)
    {
        AudioManager.Instance?.PlayJump();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, velY);
    }

    void ApplyGravityFeel()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(
                Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1f) * rb.mass
            );
        }

        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }
    }

    void UpdateCollisionStates()
    {
        Bounds bounds = col.bounds;

        // Always Compute RAW Grounded 
        Vector2 groundCenter = new Vector2(bounds.center.x, bounds.min.y - checkDistance);
        rawGrounded = Physics2D.OverlapBox(groundCenter, groundCheckSize, 0f, groundMask);

        // If we touch the Ground and not moving Up, Cancel the Ignore Grounded Lockout Immediatly
        if (rawGrounded && rb.linearVelocity.y <= 0.01f)
            ignoreGroundedTimer = 0f;

        isGrounded = rawGrounded && ignoreGroundedTimer <= 0f;

        // Walls
        Vector2 leftCenter = new Vector2(bounds.min.x - checkDistance, bounds.center.y);
        Vector2 rightCenter = new Vector2(bounds.max.x + checkDistance, bounds.center.y);

        onWallLeft = Physics2D.OverlapBox(leftCenter, wallCheckSize, 0f, groundMask);
        onWallRight = Physics2D.OverlapBox(rightCenter, wallCheckSize, 0f, groundMask);

        wallDirection = 0;
        if (!isGrounded)
        {
            if (onWallLeft) wallDirection = -1;
            else if (onWallRight) wallDirection = +1;
        }
    }

    private void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("IsGrounded", isGrounded);
        }

        if (spriteRenderer != null)
        {
            if (inputX > 0.01f)
                spriteRenderer.flipX = false;
            else if (inputX < -0.01f)
                spriteRenderer.flipX = true;
        }

        UpdateVisualSquashAndBreath();
    }

    private void UpdateVisualSquashAndBreath()
    {
        if (visualRoot == null)
            return;

        bool isIdle = isGrounded && Mathf.Abs(inputX) < 0.01f;

        Vector3 targetScale = Vector3.one;

        if (isIdle)
        {
            float breath = Mathf.Sin(Time.time * idleBreathSpeed) * idleBreathAmount;

            targetScale = new Vector3(
                1f + breath,
                1f - breath,
                1f
            );
        }

        if (!wasGrounded && isGrounded)
        {
            visualRoot.localScale = new Vector3(1.15f, 0.85f, 1f);
        }

        visualRoot.localScale = Vector3.Lerp(
            visualRoot.localScale,
            targetScale,
            Time.deltaTime * squashReturnSpeed
        );

        wasGrounded = isGrounded;
    }

    #region Inputs

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        bool isPressed = value.isPressed;

        if (isPressed)
        {
            jumpBufferTimer = jumpBufferTime;
        }
    }

    #endregion

    void OnDrawGizmosSelected()
    {
        if (!col) col = GetComponent<Collider2D>();
        if (!col) return;

        Bounds bounds = col.bounds;

        Gizmos.color = Color.green;
        Vector2 groundCenter = new Vector2(bounds.center.x, bounds.min.y - checkDistance);
        Gizmos.DrawWireCube(groundCenter, groundCheckSize);

        Gizmos.color = Color.cyan;
        Vector2 leftCenter = new Vector2(bounds.min.x - checkDistance, bounds.center.y);
        Vector2 rightCenter = new Vector2(bounds.max.x + checkDistance, bounds.center.y);
        Gizmos.DrawWireCube(leftCenter, wallCheckSize);
        Gizmos.DrawWireCube(rightCenter, wallCheckSize);
    }
}
