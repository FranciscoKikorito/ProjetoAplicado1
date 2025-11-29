using UnityEngine;
using System.Collections;

public class BetterJump : MonoBehaviour
{
    [SerializeField] private Transform player;
    [Header("Jump Settings")]
    [SerializeField] private float jumpSpeed;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;

    [Header("Dash Settings")]
    [SerializeField] public float dashDuration;
    [SerializeField] public float clickThreshold;

    private Rigidbody rb;

    private bool isGrounded;
    private bool canDash = false;
    private bool isDashing = false;

    private float clickStartTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckGround();
        Jump();
        Dash();
    }
    void CheckGround()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundLayer);

        if (!isGrounded)
            canDash = true;

        Debug.DrawRay(origin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    void Jump()
    {
        if (isGrounded && Input.GetMouseButtonDown(0))
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);

            canDash = true;
        }
    }

    void Dash()
    {
        if (isDashing) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - clickStartTime <= clickThreshold)
            {
                if (!isGrounded && canDash)
                    StartCoroutine(DoDash());
            }
            clickStartTime = Time.time;
        }
    }
    IEnumerator DoDash()
    {

        isDashing = true;
        canDash = false;

        // Obtém todas as plataformas em cena
        MovePlatform[] platforms = Object.FindObjectsByType<MovePlatform>(FindObjectsSortMode.None);

        // Acelera
        foreach (var p in platforms)
            p.SetMoveDirection(player.transform.forward * -30f);

        yield return new WaitForSeconds(dashDuration);

        // Volta ao normal
        foreach (var p in platforms)
            p.SetMoveDirection(player.transform.forward * -10f);

        isDashing = false;
    }
}