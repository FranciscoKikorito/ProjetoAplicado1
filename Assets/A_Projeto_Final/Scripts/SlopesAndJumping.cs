using UnityEngine;
using System.Collections;

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
   
    private Rigidbody rb;
    private RaycastHit hitInfo;
    private bool isGrounded = false;
    private bool isJumping = false;
    private float airborneTimer = 0f;

    [Header("Dash Settings")]
    [SerializeField] public float dashDuration = 0.2f;
    [SerializeField] public float clickThreshold = 0.25f;

    private Animator animator;
    private bool canDash = false;
    private bool isDashing = false;
    private float lastClickTime = -1f;
    private bool waitingForSecondClick = false;
    private bool pendingSingleClick = false;
    
    [Header("SFX Mecanicas")]
    public AudioSource audioSource;
    public AudioClip jumpSFX;
    public AudioClip slashSFX;

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

        // Animator sync
        if (animator != null)
        {
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isRunning", isGrounded);
        }
        //Debug.Log($"isJumping: {isJumping}, isGrounded: {isGrounded}, airborneTimer: {airborneTimer:F3}, rb.velocity.y: {rb.linearVelocity.y:F3}");
    }

    private void FixedUpdate()
    {
        // SphereCast ground check
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

        // Track air time
        if (!isGrounded)
        {
            airborneTimer += Time.fixedDeltaTime;
        }
        else
        {
            // –– OPTION A: Reset isJumping safely when touching ground
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

        // –– OPTION B: Skip ground snapping while jumping or dashing
        if (isJumping || isDashing)
            return;

        // Ground snapping
        if (isGrounded)
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
    }

    //––––––––––– CLICK LOGIC –––––––––––––//

    void Clicks()
    {
        if (GameStartController.canJump && Input.GetMouseButtonDown(0))
        {
            if (playerShield != null && playerShield.IsShieldActive()) return;
           
            if (!waitingForSecondClick)
            {
                lastClickTime = Time.time;
                waitingForSecondClick = true;
                pendingSingleClick = true;

                StartCoroutine(ClickDelay());
            }
            else
            {
                if (Time.time - lastClickTime <= clickThreshold)
                {
                    waitingForSecondClick = false;
                    pendingSingleClick = false;

                    DoDoubleClickAction();
                }
            }
        }
    }

    IEnumerator ClickDelay()
    {
        yield return new WaitForSeconds(clickThreshold);

        if (waitingForSecondClick)
        {
            waitingForSecondClick = false;

            bool stillHolding = Input.GetMouseButton(0);

            if (pendingSingleClick && !stillHolding)
                DoSingleClickAction();

            pendingSingleClick = false;
        }
    }

    //––––––––––– JUMP –––––––––––––//

    void DoSingleClickAction()
    {
        // –– OPTION C: Fix stuck state if both flags true
        if (isJumping && isGrounded)
        {
            isJumping = false;
            airborneTimer = 0f;
        }

        if (isGrounded && !isJumping)
        {
            isJumping = true;
            airborneTimer = 0f;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // play jump SFX
            if (audioSource != null && jumpSFX != null)
                audioSource.PlayOneShot(jumpSFX);
            
            if (animator != null)
                animator.SetTrigger("Jump");
        }
    }

    //––––––––––– DASH –––––––––––––//

    void DoDoubleClickAction()
    {
        if (!isGrounded && canDash && !isDashing)
        {
            // SFX do dash
            if (audioSource != null && slashSFX != null)
                audioSource.PlayOneShot(slashSFX);
            if (animator != null)
                animator.SetTrigger("Slash");

            StartCoroutine(DoDash());
        }
    }

    IEnumerator DoDash()
    {
        isDashing = true;
        canDash = false;

        MovePlatform[] platforms = Object.FindObjectsByType<MovePlatform>(FindObjectsSortMode.None);

        foreach (var p in platforms)
            p.SetMoveDirection(player.transform.forward * -30f);

        yield return new WaitForSeconds(dashDuration);

        foreach (var p in platforms)
            p.SetMoveDirection(player.transform.forward * -10f);

        isDashing = false;
    }

    //–––––– ROTATION TRIGGER ––––––//

    private Coroutine rotateCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("RotationTrigger")) return;

        Transform parent = other.transform.parent;
        if (parent == null) return;

        Transform cam = Camera.main != null ? Camera.main.transform : null;
        Quaternion camWorldRot = cam != null ? cam.rotation : Quaternion.identity;

        float parentX = parent.eulerAngles.x;
        Vector3 targetEuler = transform.eulerAngles;
        targetEuler.x = parentX;

        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(RotatePlayerSmooth(targetEuler, camWorldRot, 0.25f));
    }

    private IEnumerator RotatePlayerSmooth(Vector3 targetEuler, Quaternion camRot, float duration)
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(targetEuler);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, endRot, t / duration);

            if (Camera.main != null)
                Camera.main.transform.rotation = camRot;

            yield return null;
        }

        transform.rotation = endRot;
        if (Camera.main != null)
            Camera.main.transform.rotation = camRot;

        rotateCoroutine = null;
    }

    //–––––– GIZMOS ––––––//

    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(origin, origin + Vector3.down * sphereCastDistance);

        if (Application.isPlaying)
        {
            if (isGrounded)
            {
                Gizmos.DrawSphere(hitInfo.point, 0.05f);
                Gizmos.DrawWireSphere(origin + Vector3.down * hitInfo.distance, sphereRadius);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(origin + Vector3.down * sphereCastDistance, sphereRadius);
            }
        }
    }

    //–––––– PUBLIC CALLS ––––––//
    public bool dashCheck()
    {
        return isDashing;
    }
}
