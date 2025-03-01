using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 8f;
    public float rotationSpeed = 10f;
    
    [Header("Jump Settings")]
    public float jumpForce = 7f;
    private bool isGrounded;
    private int jumpCount = 0;
    private int maxJumps = 2; // Allows double jump

    [Header("References")]
    public Transform cam;

    private Rigidbody rb;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevents unwanted rotations
    }

    void Update()
    {
        HandleJumping();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Calculate the target angle based on camera direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            
            // Smoothly interpolate towards the target rotation
            float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
            
            // Calculate movement direction relative to the camera
            moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        // Preserve Y velocity so jumping is not overridden
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
    }

    void HandleJumping()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            jumpCount++;
            isGrounded = false; // Ensures mid-air jumps are counted correctly
        }
    }

    // Detect when player lands on the ground
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0; // Reset jumps when touching ground
        }
    }
}
