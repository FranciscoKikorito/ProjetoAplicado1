using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class SlopesAndJumping : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerShield playerShield;

    [Header("Ground Stick Settings")]
    public float desiredHeight = 0.0001f;
    public float sphereRadius = 0.25f;
    public float sphereCastDistance = 1.0f;
    public LayerMask groundLayer;
    public float snapSpeed = 20f;
    public float deadZone = 0.001f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float minAirTime = 0.08f;
    [SerializeField] private float jumpBufferTime = 0.12f;

    [Header("Dash Settings")]
    public float dashDuration = 0.2f;
    public float doubleClickThreshold = 0.25f;

    [Header("Dash Camera Effect")]
    [SerializeField] private CinemachineCamera dashCamera;
    [SerializeField] private float dashFovBoost = 15f;
    [SerializeField] private float dashFovInTime = 0.08f;
    [SerializeField] private float dashFovOutTime = 0.12f;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip jumpSFX;
    public AudioClip slashSFX;

    private Rigidbody rb;
    private Animator animator;

    private bool isGrounded;
    private bool isJumping;
    private float airborneTimer;

    // Jump buffer
    private bool jumpBuffered;
    private float jumpBufferStart;

    // Dash
    private bool canDash;
    private bool isDashing;
    private float lastClickTime;

    // Ground hit
    private RaycastHit hitInfo;

    // Rotation system
    private bool isRotating;
    private Quaternion targetRotation;
    [SerializeField] private float rotationSpeed = 360f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        animator = GetComponent<Animator>();

        if (!playerShield)
            playerShield = GetComponent<PlayerShield>();
    }

    private void Update()
    {
        HandleInput();
        ResolveBufferedJump();

        if (animator)
        {
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isRunning", isGrounded);
        }
    }

    private void FixedUpdate()
    {
        // Ground check
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        isGrounded = Physics.SphereCast(origin, sphereRadius, Vector3.down, out hitInfo, sphereCastDistance, groundLayer);

        if (isGrounded)
            canDash = true;

        if (!isGrounded)
            airborneTimer += Time.fixedDeltaTime;
        else
        {
            if (isJumping && airborneTimer >= minAirTime)
            {
                isJumping = false;
                airborneTimer = 0f;
            }
        }

        // Ground snap
        if (!isJumping && !isDashing && isGrounded)
        {
            float targetY = hitInfo.point.y + desiredHeight;
            float diff = targetY - transform.position.y;

            if (Mathf.Abs(diff) > deadZone)
            {
                Vector3 v = rb.linearVelocity;
                v.y = diff * snapSpeed;
                rb.linearVelocity = v;
            }
        }

        // Rotation
        if (isRotating)
        {
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

            if (Quaternion.Angle(rb.rotation, targetRotation) < 0.1f)
            {
                rb.MoveRotation(targetRotation);
                isRotating = false;
            }
        }
    }

    // Input handling
    private void HandleInput()
    {
        if (!GameStartController.canJump) return;

        if (Input.GetMouseButtonDown(0))
        {
            // Double click -> dash
            if (Time.time - lastClickTime <= doubleClickThreshold)
            {
                TryDash();
                lastClickTime = 0f;
                return;
            }

            lastClickTime = Time.time;

            // Buffer jump
            BufferJump();
        }
    }

    private void BufferJump()
    {
        if (!isGrounded || isJumping) return;

        jumpBuffered = true;
        jumpBufferStart = Time.time;

        if (animator)
            animator.SetTrigger("Jump");
    }

    private void ResolveBufferedJump()
    {
        if (!jumpBuffered) return;

        // Shield cancels jump
        if (playerShield && playerShield.IsShieldActive())
        {
            jumpBuffered = false;
            return;
        }

        // Jump on release or timeout
        if (!Input.GetMouseButton(0) || Time.time - jumpBufferStart >= jumpBufferTime)
        {
            ExecuteJump();
            jumpBuffered = false;
        }
    }

    private void ExecuteJump()
    {
        isJumping = true;
        airborneTimer = 0f;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (audioSource && jumpSFX)
            audioSource.PlayOneShot(jumpSFX);
    }

    // Dash
    private void TryDash()
    {
        if (isGrounded || !canDash || isDashing) return;

        if (audioSource && slashSFX)
            audioSource.PlayOneShot(slashSFX);

        if (animator)
            animator.SetTrigger("Slash");

        StartCoroutine(DoDash());
    }

    private IEnumerator DoDash()
    {
        isDashing = true;
        canDash = false;

        float baseFov = dashCamera ? dashCamera.Lens.FieldOfView : 0f;

        if (dashCamera)
            StartCoroutine(FovDash(baseFov, baseFov + dashFovBoost));

        MovePlatform[] platforms = Object.FindObjectsByType<MovePlatform>(FindObjectsSortMode.None);
        foreach (var p in platforms)
            p.SetMoveDirection(player.transform.forward * -30f);

        yield return new WaitForSeconds(dashDuration);

        foreach (var p in platforms)
            p.SetMoveDirection(player.transform.forward * -10f);

        if (dashCamera)
            StartCoroutine(FovDash(baseFov + dashFovBoost, baseFov));

        isDashing = false;
    }

    private IEnumerator FovDash(float from, float to)
    {
        float duration = from < to ? dashFovInTime : dashFovOutTime;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            dashCamera.Lens.FieldOfView = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        dashCamera.Lens.FieldOfView = to;
    }

    // Rotation triggers
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("RotationTrigger")) return;

        Transform parent = other.transform.parent;
        if (parent == null) return;

        Vector3 euler = rb.rotation.eulerAngles;
        euler.x = parent.eulerAngles.x;

        targetRotation = Quaternion.Euler(euler);
        isRotating = true;
    }

    // Dash check
    public bool dashCheck() => isDashing;


    public bool checkGrounded()
    {
        return isGrounded;
    }
}
