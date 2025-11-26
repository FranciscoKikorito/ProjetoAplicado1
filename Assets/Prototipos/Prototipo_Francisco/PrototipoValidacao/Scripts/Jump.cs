using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        bool isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }

        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }
}
