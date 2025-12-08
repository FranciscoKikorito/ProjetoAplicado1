using UnityEngine;
using System.Collections;
public class PlayerLives: MonoBehaviour
{
    public int lives ;

    [Header("Flash Effect")]
    public float flashDuration ;
    public int flashCount;
    public Material flashMaterial; 
    public Transform playerModel;
    private Material[] originalMaterials;
    private Renderer[] renderers;
    
    [Header("Animator")]
    public Animator animator;
    private bool isDead = false;
    
    [Header("Plataformas")]
    public MovePlatform[] allPlatforms;
    public float platformStartSpeed = -10f;
    [SerializeField] private float reviveDelay = 0.5f;
    
    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip hitByLaserSFX;
    void Awake()
    {
        renderers = playerModel.GetComponentsInChildren<Renderer>();
        
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalMaterials[i] = renderers[i].material;
        
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    public void ApplyDamage(int amount)
    {
        lives -= amount;
        Debug.Log("Player levou dano. Vidas restantes: " + lives);
        if (audioSource && hitByLaserSFX)
            audioSource.PlayOneShot(hitByLaserSFX);

        StartCoroutine(FlashEffect());

        if (lives <= 0)
            Die();
    }

    IEnumerator FlashEffect()
    {
        for (int i = 0; i < flashCount; i++)
        {
            SetAllMaterials(flashMaterial);
            yield return new WaitForSeconds(flashDuration);
            RestoreMaterials();
            yield return new WaitForSeconds(flashDuration);
        }
    }
    void SetAllMaterials(Material m)
    {
        foreach (Renderer r in renderers)
            r.material = m;
    }
    void RestoreMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material = originalMaterials[i];
    }
    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player morreu!");

        // Para as plataformas
        foreach (var p in allPlatforms) 
            p.enabled = false;
        
        if (animator != null)
            animator.SetTrigger("Die");
        
        StartCoroutine(StandUpAndResumePlatforms());
    }
    IEnumerator StandUpAndResumePlatforms()
    {
        // Pequeno delay antes de levantar
        yield return new WaitForSeconds(0.5f);
        
        if (animator != null)
            animator.Play("StandUp");

        // Espera até que a animação termine
        if (animator != null)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            float duration = state.length;

            // Se quiser garantir que a transição foi para StandUp
            while (!state.IsName("StandUp"))
            {
                yield return null;
                state = animator.GetCurrentAnimatorStateInfo(0);
            }

            // Espera a duração completa da animação
            yield return new WaitForSeconds(state.length);
        }

        // Reativa movimento das plataformas
        foreach (var p in allPlatforms)
            p.enabled = true;
        
        // Resetar vidas 
        lives = Mathf.Max(lives, 2);
        isDead = false;
    }
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