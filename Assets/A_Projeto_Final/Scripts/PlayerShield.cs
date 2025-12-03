using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [Header("Referências")]
    public Transform shieldObject; 
    public Collider shieldCollider;

    [Header("Configuração")]
    public float holdThreshold = 0.25f;
    public Vector3 shieldOffset = new Vector3(0, 1.0f, 1.0f);

    private bool shieldActive = false;
    private float lmbDownTime;
    private bool isHolding = false;
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        
        if (shieldObject != null)
            shieldObject.gameObject.SetActive(false);

        if (shieldCollider == null && shieldObject != null)
            shieldCollider = shieldObject.GetComponent<Collider>();
        
        if (shieldCollider != null)
        {
            Collider[] playerColliders = GetComponentsInChildren<Collider>(true);

            foreach (Collider col in playerColliders)
            {
                if (col != shieldCollider)
                    Physics.IgnoreCollision(shieldCollider, col);
            }
        }

        if (shieldObject != null)
            shieldObject.gameObject.layer = LayerMask.NameToLayer("Shield");
    }

    void Update()
    {
        HandleShieldInput();

        if (shieldActive)
            UpdateShieldPosition();
    }

    private void HandleShieldInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lmbDownTime = Time.time;
            isHolding = true;
        }

        if (isHolding && Input.GetMouseButton(0))
        {
            if (!shieldActive && Time.time - lmbDownTime > holdThreshold)
                ToggleShield(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            if (shieldActive)
                ToggleShield(false);
        }
    }

    private void UpdateShieldPosition()
    {
        if (shieldObject == null) return;

        shieldObject.position = transform.position + transform.TransformDirection(shieldOffset);
        shieldObject.rotation = transform.rotation;
    }

    public void ToggleShield(bool state)
    {
        shieldActive = state;

        if (shieldObject != null)
            shieldObject.gameObject.SetActive(state);
        if (animator != null)
            animator.SetBool("isShielding", state);
    }

    public bool IsShieldActive() => shieldActive;

    public bool IsHoldingForShield()
    {
        return isHolding && !shieldActive && (Time.time - lmbDownTime < holdThreshold);
    }
}