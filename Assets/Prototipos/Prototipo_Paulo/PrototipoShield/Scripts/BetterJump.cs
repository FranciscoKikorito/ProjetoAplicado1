using UnityEngine;
using System.Collections;

public class BetterJump : MonoBehaviour
{
    [SerializeField] private Transform player; 
    [Header("Jump Settings")]
    [SerializeField] private float jumpSpeed = 10f;
    
    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.4f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Dash Settings")]
    public float dashDuration = 0.15f;
    public float clickThreshold = 0.2f;

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
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance,groundLayer);

        if (isGrounded)
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
            clickStartTime = Time.time;

        if (Input.GetMouseButtonUp(0))
        {
            float clickDuration = Time.time - clickStartTime;

            if (!isGrounded && canDash && clickDuration <= clickThreshold)
            {
                StartCoroutine(DoDash());
            }
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