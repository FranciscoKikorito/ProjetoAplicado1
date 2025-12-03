using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SlopesAndJumping : MonoBehaviour
{
    [Header("Ground Stick Settings")]
    public float desiredHeight = 0.1f;
    public float sphereRadius = 0.25f;
    public float sphereCastDistance = 1.0f;
    public LayerMask groundLayer;
    public float snapSpeed = 20f;
    public float deadZone = 0.01f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float minAirTime = 0.08f; // small buffer to avoid instant re-enable on the same frame

    private Rigidbody rb;
    private RaycastHit hitInfo;
    private bool grounded = false;
    private bool isJumping = false;
    private float airborneTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        // optional: make collision detection more stable for fast movement
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void Update()
    {
        // Read input in Update for responsiveness — use the grounded cached from last FixedUpdate
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            // Start jump
            isJumping = true;
            airborneTimer = 0f;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        // Ground check via spherecast (origin slightly above to avoid starting inside ground)
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        grounded = Physics.SphereCast(
            origin,
            sphereRadius,
            Vector3.down,
            out hitInfo,
            sphereCastDistance,
            groundLayer
        );

        // Track air time while not grounded
        if (!grounded)
        {
            airborneTimer += Time.fixedDeltaTime;
        }
        else
        {
            // If we are grounded and we were jumping, only clear jumping state when:
            // - we're moving down or stationary (rb.velocity.y <= small threshold)
            // - AND we have been in the air for at least minAirTime (avoids immediate re-enable)
            if (isJumping && rb.linearVelocity.y <= 0.05f && airborneTimer >= minAirTime)
            {
                isJumping = false;
                airborneTimer = 0f;
            }
            else if (!isJumping)
            {
                // ensure airborneTimer reset when naturally grounded
                airborneTimer = 0f;
            }
        }

        // If currently jumping (in the air from our input), DO NOT apply ground sticking
        if (isJumping)
            return;

        // Apply ground-stick only when grounded and not jumping
        if (grounded)
        {
            float targetY = hitInfo.point.y + desiredHeight;
            float difference = targetY - transform.position.y;

            if (Mathf.Abs(difference) > deadZone)
            {
                Vector3 vel = rb.linearVelocity;
                // Only affect vertical velocity — reduce risk of fighting horizontal motion
                vel.y = difference * snapSpeed;
                rb.linearVelocity = vel;
            }
        }
    }

    //START OF TRIGGER
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

        if (rotateCoroutine != null) StopCoroutine(rotateCoroutine);
        rotateCoroutine = StartCoroutine(RotatePlayerSmooth(targetEuler, camWorldRot, 0.25f)); // 0.25s duration
    }

    private System.Collections.IEnumerator RotatePlayerSmooth(Vector3 targetEuler, Quaternion camWorldRot, float duration)
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(targetEuler);
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, endRot, t / duration);

            // keep camera world rotation stable during the rotation
            if (Camera.main != null) Camera.main.transform.rotation = camWorldRot;

            yield return null;
        }
        transform.rotation = endRot;
        if (Camera.main != null) Camera.main.transform.rotation = camWorldRot;
        rotateCoroutine = null;
    }
    //END OF TRIGGER


    private void OnDrawGizmos()
    {
        // Show gizmos even while edit-mode may be useful, but protect hitInfo usage
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.color = grounded ? Color.green : Color.red;
        Gizmos.DrawLine(origin, origin + Vector3.down * sphereCastDistance);

        if (Application.isPlaying)
        {
            if (grounded)
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
}
