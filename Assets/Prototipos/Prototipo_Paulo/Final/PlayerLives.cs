using UnityEngine;
using System.Collections;

public class PlayerLives : MonoBehaviour
{
    public int lives;

    [Header("Flash Effect")]
    public float flashDuration = 0.1f;
    public int flashCount = 5;
    public Material flashMaterial;
    public Transform playerModel;
    
    // Cache dos materiais originais para restaurar depois
    private Material[][] originalMaterials; 
    private Renderer[] renderers;

    [Header("Animator")]
    public Animator animator;
    private bool isDead = false;

    [Header("Plataformas")]
    public MovePlatform[] allPlatforms;
    public float platformStartSpeed = -10f;
    [SerializeField] private float reviveDelay = 0.5f;

    [Header("Invencibilidade")]
    public float invincibilityDuration = 3.0f;
    // Tornei publico caso outros scripts precisem de saber se estás invencível
    public bool isInvincible = false; 

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip hitByLaserSFX;

    void Awake()
    {
        // Se não arrastaste nada para o playerModel, usa o próprio transform
        if (playerModel == null) playerModel = transform;

        renderers = playerModel.GetComponentsInChildren<Renderer>();

        // Guardamos a lista de materiais de cada renderer (alguns renderers têm mais de 1 material)
        originalMaterials = new Material[renderers.Length][];
        
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].sharedMaterials;
        }

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void ApplyDamage(int amount)
    {
        if (isDead || isInvincible) return;

        lives -= amount;
        Debug.Log("Player levou dano. Vidas restantes: " + lives);

        if (audioSource && hitByLaserSFX)
            audioSource.PlayOneShot(hitByLaserSFX);

        // Se tiveres um material de flash definido, faz o efeito
        if (flashMaterial != null)
        {
            StopCoroutine("FlashEffect"); // Garante que não acumula efeitos
            StartCoroutine("FlashEffect");
        }

        // Inicia invencibilidade
        StartCoroutine(InvincibilityRoutine());

        // if (lives <= 0) Die();
    }

    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        // Debug.Log("Player Invencível!");

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
        // Debug.Log("Player vulnerável novamente.");
    }

    IEnumerator FlashEffect()
    {
        for (int i = 0; i < flashCount; i++)
        {
            // Troca para o material de Flash (Branco/Vermelho)
            SetFlashMaterial();
            yield return new WaitForSeconds(flashDuration);
            
            // Restaura a cor original
            RestoreMaterials();
            yield return new WaitForSeconds(flashDuration);
        }
    }

    void SetFlashMaterial()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            int matCount = renderers[i].sharedMaterials.Length;
            Material[] flashes = new Material[matCount];
            
            // Preenche o array com o material de flash
            for (int k = 0; k < matCount; k++)
            {
                flashes[k] = flashMaterial;
            }
            
            renderers[i].materials = flashes;
        }
    }

    void RestoreMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            // Restaura os materiais originais guardados no Awake
            renderers[i].materials = originalMaterials[i];
        }
    }

    /*
     * MÉTODOS DE COLISÃO
     * Se o boneco parar de rodar ao bater na parede, 
     * o problema é a fricção (Physics Material) da parede ou do player,
     * não o script.
     */
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Wall") && !other.collider.CompareTag("Shield"))
        {
            ApplyDamage(1);
        }

        if (other.collider.CompareTag("Obstacle"))
        {
            ApplyDamage(1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") && !other.CompareTag("Shield"))
        {
            ApplyDamage(1);
        }

        if (other.CompareTag("Obstacle"))
        {
            ApplyDamage(1);
        }
    }
}