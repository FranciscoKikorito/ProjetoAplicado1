using UnityEngine;
using System.Collections;

public class PlayerLives : MonoBehaviour
{
    public int lives = 3;

    [Header("Flash Effect")]
    public float flashDuration = 0.1f;
    public int flashCount = 5;
    public Material flashMaterial;
    public Transform playerModel;
    private Material[][] originalMaterials;
    private Renderer[] renderers;

    [Header("Animator")]
    public Animator animator;
    private bool isDead = false;

    [Header("Plataformas")]
    public MovePlatform[] allPlatforms;
    public float platformStartSpeed = -10f;
    //[SerializeField] private float reviveDelay = 0.5f;

    [Header("GameOver References")]
    public GameStartController gameController;

    [Header("Invencibilidade")]
    public float invincibilityDuration = 3.0f;

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
        if (gameController == null)
            gameController = FindObjectOfType<GameStartController>();
    }

    public void ApplyDamage(int amount)
    {
        if (isDead || isInvincible) return;

        lives -= amount;
        Debug.Log("Player levou dano. Vidas restantes: " + lives);

        if (audioSource && hitByLaserSFX)
            audioSource.PlayOneShot(hitByLaserSFX);

        if (lives <= 0)
        {
            Die();
        }
        else
        {
            if (flashMaterial != null)
            {
                StopCoroutine("FlashEffect");
                StartCoroutine("FlashEffect");
            }
            StartCoroutine(InvincibilityRoutine());
        }

    }
    void Die()
    {
        isDead = true;
        //se quisermos colocar animacao aqui.
        //if(animator != null) animator.SetTrigger("Die"); 
        Debug.Log("Game Over!");

        if (gameController != null)
        {
            gameController.TriggerGameOver();
        }
    }
    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    IEnumerator FlashEffect()
    {
        for (int i = 0; i < flashCount; i++)
        {
            SetFlashMaterial();
            yield return new WaitForSeconds(flashDuration);

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
            renderers[i].materials = originalMaterials[i];
        }
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