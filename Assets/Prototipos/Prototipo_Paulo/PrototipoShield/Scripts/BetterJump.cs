using UnityEngine;
using System.Collections;

public class BetterJump : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerShield playerShield; 

    [Header("Jump Settings")]
    [SerializeField] private float jumpSpeed;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;

    [Header("Dash Settings")]
    [SerializeField] public float dashDuration;
    [SerializeField] public float clickThreshold; 
    private Rigidbody rb;
    private Animator animator;
    
    private bool isGrounded;
    private bool canDash = false;
    private bool isDashing = false;

    private float lastClickTime = -1f;
    private bool waitingForSecondClick = false;

    private bool pendingSingleClick = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        if (playerShield == null)
            playerShield = GetComponent<PlayerShield>();
    }

    void Update()
    {
        CheckGround();
        if (animator != null)
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isRunning", isGrounded);
        Clicks();
    }

    void CheckGround()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        
        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded)
        {
            canDash = true;
        }
    }

    void Clicks()
    {
        if (Input.GetMouseButtonDown(0))
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
            
            bool isHoldingButton = Input.GetMouseButton(0); 

            if (pendingSingleClick && !isHoldingButton)
            {
                DoSingleClickAction();
            }

            pendingSingleClick = false;
        }
    }

    void DoSingleClickAction()
    {
        
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            if (animator != null)
                animator.SetTrigger("Jump");
        }
    }

    void DoDoubleClickAction()
    {
        if (!isGrounded && canDash && !isDashing)
        {
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
}