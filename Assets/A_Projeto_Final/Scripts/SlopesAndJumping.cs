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

    [Header("Dash Settings")]
    [SerializeField] public float dashDuration = 0.2f;
    [SerializeField] public float clickThreshold = 0.25f;

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

    private RaycastHit hitInfo;
    private bool isGrounded;
    private bool isJumping;
    private float airborneTimer;

    private bool canDash;
    private bool isDashing;

    private float lastClickTime;
    private bool waitingForSecondClick;
    private bool pendingSingleClick;

    //–––––––– ROTATION STATE (PHYSICS SAFE) ––––––––
    private bool isRotating;
    private Quaternion targetRotation;
    [SerializeField] private float rotationSpeed = 360f; // degrees per second

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        animator = GetComponent<Animator>();

        if (playerShield == null)
            playerShield = GetComponent<PlayerShield>();
    }

    private void Update()
    {
        Clicks();

        if (animator != null)
        {
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isRunning", isGrounded);
        }
    }

    private void FixedUpdate()
    {
        //––––– GROUND CHECK –––––//
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        isGrounded = Physics.SphereCast(
            origin,
            sphereRadius,
            Vector3.down,
            out hitInfo,
            sphereCastDistance,
            groundLayer
        );

        if (isGrounded)
            canDash = true;

        //––––– AIR TIMER –––––//
        if (!isGrounded)
        {
            airborneTimer += Time.fixedDeltaTime;
        }
        else
        {
            if (isJumping && airborneTimer >= minAirTime)
            {
                isJumping = false;
                airborneTimer = 0f;
            }
            else if (!isJumping)
            {
                airborneTimer = 0f;
            }
        }

        // Skip snapping during jump / dash
        if (!isJumping && !isDashing && isGrounded)
        {
            float targetY = hitInfo.point.y + desiredHeight;
            float diff = targetY - transform.position.y;

            if (Mathf.Abs(diff) > deadZone)
            {
                Vector3 vel = rb.linearVelocity;
                vel.y = diff * snapSpeed;
                rb.linearVelocity = vel;
            }
        }

        //––––– PHYSICS-SAFE ROTATION –––––//
        if (isRotating)
        {
            rb.MoveRotation(
                Quaternion.RotateTowards(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                )
            );

            if (Quaternion.Angle(rb.rotation, targetRotation) < 0.1f)
            {
                rb.MoveRotation(targetRotation);
                isRotating = false;
            }
        }
    }

    //–––––––– CLICK LOGIC ––––––––//
    void Clicks()
    {
        if (!GameStartController.canJump) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (playerShield != null && playerShield.IsShieldActive()) return;

        if (!waitingForSecondClick)
        {
            lastClickTime = Time.time;
            waitingForSecondClick = true;
            pendingSingleClick = true;
            StartCoroutine(ClickDelay());
        }
        else if (Time.time - lastClickTime <= clickThreshold)
        {
            waitingForSecondClick = false;
            pendingSingleClick = false;
            DoDoubleClickAction();
        }
    }

    IEnumerator ClickDelay()
    {
        yield return new WaitForSeconds(clickThreshold);

        if (waitingForSecondClick)
        {
            waitingForSecondClick = false;

            if (pendingSingleClick && !Input.GetMouseButton(0))
                DoSingleClickAction();

            pendingSingleClick = false;
        }
    }

    //–––––––– JUMP ––––––––//
    void DoSingleClickAction()
    {
        if (isGrounded && !isJumping)
        {
            isJumping = true;
            airborneTimer = 0f;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (audioSource && jumpSFX)
                audioSource.PlayOneShot(jumpSFX);

            if (animator)
                animator.SetTrigger("Jump");
        }
    }

    //–––––––– DASH ––––––––//
    void DoDoubleClickAction()
    {
        if (!isGrounded && canDash && !isDashing)
        {
            if (audioSource && slashSFX)
                audioSource.PlayOneShot(slashSFX);

            if (animator)
                animator.SetTrigger("Slash");

            StartCoroutine(DoDash());
        }
    }

    IEnumerator DoDash()
    {
        isDashing = true;
        canDash = false;

        float baseFov = dashCamera != null ? dashCamera.Lens.FieldOfView : 0f;

        if (dashCamera)
            StartCoroutine(FovDash(baseFov, baseFov + dashFovBoost));

        MovePlatform[] platforms =
            Object.FindObjectsByType<MovePlatform>(FindObjectsSortMode.None);

        foreach (var p in platforms)
            p.SetMoveDirection(player.transform.forward * -30f);

        yield return new WaitForSeconds(dashDuration);

        foreach (var p in platforms)
            p.SetMoveDirection(player.transform.forward * -10f);

        if (dashCamera)
            StartCoroutine(FovDash(baseFov + dashFovBoost, baseFov));

        isDashing = false;
    }

    IEnumerator FovDash(float from, float to)
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

    //–––––– ROTATION TRIGGER ––––––//
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

    //–––––– GIZMOS ––––––//
    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(origin, origin + Vector3.down * sphereCastDistance);
    }

    //–––––– PUBLIC ––––––//
    public bool dashCheck()
    {
        return isDashing;
    }
}
