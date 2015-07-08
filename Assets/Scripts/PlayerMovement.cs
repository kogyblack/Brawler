using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour {
    Rigidbody2D theRigidbody2D;
    Collider2D theCollider2D;

    // Ground check
    public Vector2 groundCheckA, groundCheckB;
    // TODO(naum): Center this information into a Manager class
    public LayerMask groundLayer;

    // Player Keys
    public KeyCode upKey    = KeyCode.W;
    public KeyCode downKey  = KeyCode.S;
    public KeyCode leftKey  = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey  = KeyCode.Space;

    public bool haveControl = true;

    // Ground Checking
    public bool isGrounded = false;
    const float groundCheckMargin = 3f/32f; // 3 pixels

    public bool isJumping = false;
    public int airJumps = 1;
    [SerializeField] private int airJumpCount = 0;

    // Speed Cap
    public float acceleration = 5f, deacceleration = 8f;
    public float jumpSpeed = 10f;
    public float airJumpSpeed = 8f;
    public float maxSpeed = 3f;
    public float velocityThreshold = 0.1f;

    public float terminalSpeed = 12f;
    public bool limitTerminalSpeed = true;

    // Debug
    public Vector2 velocity;

    void Awake() {
        theRigidbody2D = GetComponent<Rigidbody2D>();
        theCollider2D = GetComponent<Collider2D>();
    }

    void Start() {
        groundCheckA = new Vector2(-theCollider2D.bounds.extents.x, -theCollider2D.bounds.extents.y);
        groundCheckB = new Vector2( theCollider2D.bounds.extents.x, -theCollider2D.bounds.extents.y);
    }

    void Update() {
        if (theRigidbody2D.velocity.y <= 0f) {
            isGrounded = Physics2D.OverlapArea(
                groundCheckA + theRigidbody2D.position,
                groundCheckB + theRigidbody2D.position - Vector2.up * groundCheckMargin,
                groundLayer);
            if (isGrounded) {
                airJumpCount = 0;
            }
        }

        // Inputs
        if (Input.GetKeyDown(jumpKey)) {
            isJumping = true;
        }
    }

    void FixedUpdate() {
        if (haveControl) {
            Vector2 targetVelocity = theRigidbody2D.velocity;

            InputMovement(ref targetVelocity);
            MovementCap(ref targetVelocity);

            theRigidbody2D.velocity = targetVelocity;
            velocity = targetVelocity;
        }
    }

    void InputMovement(ref Vector2 targetVelocity) {
        if (Input.GetKey(rightKey) && !Input.GetKey(leftKey) && targetVelocity.x >= 0f) {
            targetVelocity.x += acceleration * Time.fixedDeltaTime;
        } else if (!Input.GetKey(rightKey) && Input.GetKey(leftKey) && targetVelocity.x <= 0f) {
            targetVelocity.x -= acceleration * Time.fixedDeltaTime;
        } else {
            float sign = Mathf.Sign(targetVelocity.x);
            targetVelocity.x -= Mathf.Sign(targetVelocity.x) * deacceleration * Time.fixedDeltaTime;
            if (Mathf.Sign(targetVelocity.x) != sign)
                targetVelocity.x = 0f;
        }

        if (isJumping) {
            if (isGrounded) {
                // Ground Jump
                targetVelocity.y = jumpSpeed;
                isGrounded = false;
            } else if (airJumpCount < airJumps) {
                // Air Jump
                targetVelocity.y = airJumpSpeed;
                airJumpCount++;
            }
            isJumping = false;
        }
    }

    void MovementCap(ref Vector2 targetVelocity) {
        // Max Horizontal Speed
        if (Mathf.Abs(targetVelocity.x) > maxSpeed) {
            targetVelocity.x = Mathf.Sign(targetVelocity.x) * maxSpeed;
        }

        // Max Dropping Speed
        if (limitTerminalSpeed && targetVelocity.y < -terminalSpeed) {
            targetVelocity.y = -terminalSpeed;
        }
    }
}
