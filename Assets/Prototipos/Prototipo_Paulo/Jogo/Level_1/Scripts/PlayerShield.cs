using UnityEngine;
using System.Collections;

public class PlayerShield : MonoBehaviour
{
    [Header("Referências")]
    public GameObject shieldObject;          // o GameObject do shield (filho, com MeshRenderer ou VFX)
    public Collider shieldCollider;          // o collider do shield (no filho)
    //public PlayerSelfController playerController; 

    [Header("Configuração")]
    public float holdThreshold = 0.25f;      // tempo mínimo a segurar LMB para ativar

    [Header("Estados")]
    private bool shieldActive = false;
    private float lmbDownTime;
    private bool isHolding = false;

    void Start()
    {
        // encontra o PlayerSelfController no mesmo objeto
        //if (playerController == null)
        //   playerController = GetComponent<PlayerSelfController>();

        // encontra automaticamente o shield (filho)
        if (shieldObject == null)
            shieldObject = transform.Find("Shield")?.gameObject;

        // tenta encontrar o collider no filho
        if (shieldObject != null && shieldCollider == null)
            shieldCollider = shieldObject.GetComponent<Collider>();

        // garante que o shield começa desativado
        if (shieldObject != null)
            shieldObject.SetActive(false);

        // ignora colisões com os jogadores (White e Black)
        if (shieldCollider != null)
        {
            Collider[] playerColliders = GetComponentsInChildren<Collider>();
            foreach (Collider col in playerColliders)
            {
                if (col != shieldCollider)
                    Physics.IgnoreCollision(shieldCollider, col);
            }
        }

        if (shieldObject != null)
            shieldObject.layer = LayerMask.NameToLayer("Shield");
    }

    void Update()
    {
        //if (playerController == null || !playerController.IsWhiteActive())
        //{
        //if (shieldActive)
        //{
        //    ToggleShield(false);
        //}
        //}

        // Pressionar LMB
        if (Input.GetMouseButtonDown(0))
        {
            lmbDownTime = Time.time;
            isHolding = true;
        }

        // Segurar LMB por tempo suficiente → ativa
        if (isHolding && Input.GetMouseButton(0))
        {
            if (!shieldActive && Time.time - lmbDownTime > holdThreshold)
                ToggleShield(true);
        }

        // Soltar LMB → desativa
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
