using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [Header("Referências")]
    public GameObject shieldObject;     // Objeto visual do escudo (filho do capsule)
    public Collider shieldCollider;     // Collider do escudo (no filho)

    [Header("Configuração")]
    public float holdThreshold = 0.25f; // Tempo mínimo de segurar o botão antes de ativar

    private bool shieldActive = false;
    private float lmbDownTime;
    private bool isHolding = false;

    void Start()
    {
        // Tenta encontrar automaticamente o escudo no filho
        if (shieldObject == null)
            shieldObject = transform.Find("Shield")?.gameObject;

        // Pega o collider do escudo
        if (shieldObject != null && shieldCollider == null)
            shieldCollider = shieldObject.GetComponent<Collider>();

        // Desativa o escudo no início
        if (shieldObject != null)
            shieldObject.SetActive(false);

        // Garante que o escudo não colide com o corpo do jogador (capsule)
        if (shieldCollider != null)
        {
            Collider playerCollider = GetComponent<Collider>();
            if (playerCollider != null)
                Physics.IgnoreCollision(shieldCollider, playerCollider);
        }

        // Define a layer "Shield" (caso exista)
        if (shieldObject != null)
            shieldObject.layer = LayerMask.NameToLayer("Shield");
    }

    void Update()
    {
        // Pressionar LMB → começa a contar o tempo
        if (Input.GetMouseButtonDown(0))
        {
            lmbDownTime = Time.time;
            isHolding = true;
        }

        // Segurar LMB o tempo suficiente → ativa o escudo
        if (isHolding && Input.GetMouseButton(0))
        {
            if (!shieldActive && Time.time - lmbDownTime > holdThreshold)
                ToggleShield(true);
        }

        // Soltar LMB → desativa o escudo
        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            if (shieldActive)
                ToggleShield(false);
        }
    }

    void ToggleShield(bool state)
    {
        shieldActive = state;

        if (shieldObject != null)
            shieldObject.SetActive(state);
    }

    public bool IsShieldActive()
    {
        return shieldActive;
    }
}