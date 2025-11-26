using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class JumpObstacles : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce = 10f;
    public float gravity = -20f;

    [Header("Dash")]
    public float dashDuration = 0.15f;
    private bool canDash = false;
    private bool isDashing = false;

    [Header("Input")]
    public float clickThreshold = 0.2f; 
    public LayerMask groundLayer;

    private CharacterController controller;
    //private Animator anim;
    private float verticalVelocity = 0f;
    private float clickStartTime = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        //anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        bool grounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            controller.height / 2 + 0.2f,
            groundLayer
        );

        HandleInput(grounded);

        if (!isDashing)
            ApplyGravity(grounded);

        MovePlayer();
    }

    void HandleInput(bool grounded)
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickStartTime = Time.time;
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            float clickDuration = Time.time - clickStartTime;
            
            if (clickDuration <= clickThreshold && grounded)
            {
                verticalVelocity = jumpForce;
                canDash = true;       // permite dash depois de saltar
                return;
            }
            
            if (clickDuration <= clickThreshold && !grounded && canDash)
            {
                StartCoroutine(DoDash());
            }
        }
    }
    IEnumerator DoDash()
    {
        isDashing = true;
        canDash = false;
        
        //anim.SetTrigger("JumpKick");
        
        // Apanha todas as plataformas existentes
        PlatformMove[] platforms = Object.FindObjectsByType<PlatformMove>(FindObjectsSortMode.None);

        // Ativa dash nelas
        foreach (var p in platforms)
            p.SetDash(true);

        yield return new WaitForSeconds(dashDuration);

        // Desativa dash
        foreach (var p in platforms)
            p.SetDash(false);

        isDashing = false;
    }

    void ApplyGravity(bool grounded)
    {
        if (!grounded || verticalVelocity > 0)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        else
        {
            verticalVelocity += gravity * 2f * Time.deltaTime;
        }
    }

    void MovePlayer()
    {
        controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }
    //void OnControllerColliderHit(ControllerColliderHit hit)
    //{
      // if (isDashing && hit.collider.CompareTag("Wall"))
       //{
        // Destroy(hit.collider.gameObject);
       //}
   //}
}