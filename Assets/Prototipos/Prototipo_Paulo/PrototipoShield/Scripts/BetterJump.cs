using UnityEngine;
using System.Collections;

public class BetterJump : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerShield playerShield; // <<< ARRASTA O SCRIPT DO PLAYER SHIELD PARA AQUI

    [Header("Jump Settings")]
    [SerializeField] private float jumpSpeed;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;

    [Header("Dash Settings")]
    [SerializeField] public float dashDuration;
    [SerializeField] public float clickThreshold; // Deve ser menor que o holdThreshold do Shield (ex: 0.2s)

    private Rigidbody rb;

    private bool isGrounded;
    private bool canDash = false;
    private bool isDashing = false;

    private float lastClickTime = -1f;
    private bool waitingForSecondClick = false;

    private bool pendingSingleClick = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (playerShield == null)
            playerShield = GetComponent<PlayerShield>();
    }

    void Update()
    {
        CheckGround();
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
                    pendingSingleClick = false;   // Cancela salto simples
                    DoDoubleClickAction();        // Executa Dash
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
        }
    }

    void DoDoubleClickAction()
    {
        if (!isGrounded && canDash && !isDashing)
            StartCoroutine(DoDash());
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