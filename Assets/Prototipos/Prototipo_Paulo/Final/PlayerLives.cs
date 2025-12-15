using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlayerLives : MonoBehaviour
{
    public int lives = 3;
    
    [Header("UI Vidas")]
    public Image[] heartImages; // 3 corações
    public GameObject livesUI;  // objeto pai (LivesUI)
    public float heartFlashDuration = 0.1f;
    public int heartFlashCount = 4;
    public float uiVisibleAfterHitTime = 1.5f;
    public string runStateName = "Rig|Run";
    
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
        if (playerModel == null) playerModel = transform;

        renderers = playerModel.GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].sharedMaterials;
        }

        if (animator == null)
            animator = GetComponent<Animator>();
        if (gameController == null)
            gameController = FindObjectOfType<GameStartController>();
        
        if (animator != null)
        {
            StartCoroutine(WaitForRunAndShowLives());
        }
        if (livesUI != null)
        {
            livesUI.SetActive(false);
        }
    }
    public void ApplyDamage(int amount)
    {
        if (isDead || isInvincible) return;
        
        int previousLives = lives;
        lives -= amount;
        Debug.Log("Player levou dano. Vidas restantes: " + lives);
        
        if (audioSource && hitByLaserSFX)
            audioSource.PlayOneShot(hitByLaserSFX);

        livesUI.SetActive(true);
        UpdateLivesUI(previousLives);

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
            StartCoroutine(HideLivesUIAfterDelay());
        }

    }
    void UpdateLivesUI(int previousLives)
    {
        int lostHeartIndex = previousLives - 1;

        if (lostHeartIndex >= 0 && lostHeartIndex < heartImages.Length)
        {
            StartCoroutine(FlashHeart(lostHeartIndex));
        }
    }
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        isInvincible = true;
        
        GameStartController.inputLocked = true;
        GameStartController.canJump = false;
        
        if(animator != null) animator.SetTrigger("Die"); 
        
        Debug.Log("Game Over!");
        if (gameController != null)
        {
            gameController.TriggerGameOver();
        }
    }
    void SetAllHeartsVisible(bool visible)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = visible;
        }
    }
    IEnumerator WaitForRunAndShowLives()
    {
        // Espera até o Animator entrar no estado de Run
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(runStateName))
        {
            yield return null;
        }

        // Agora sim começa o UI
        yield return StartCoroutine(ShowLivesAtStart());
    }
    IEnumerator ShowLivesAtStart()
    {
        livesUI.SetActive(true);
        
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = true;
        }
        
        for (int i = 0; i < heartFlashCount; i++)
        {
            SetAllHeartsVisible(false);
            yield return new WaitForSeconds(heartFlashDuration);

            SetAllHeartsVisible(true);
            yield return new WaitForSeconds(heartFlashDuration);
        }

        yield return new WaitForSeconds(2f);

        livesUI.SetActive(false);
    }
    IEnumerator HideLivesUIAfterDelay()
    {
        yield return new WaitForSeconds(uiVisibleAfterHitTime);
        livesUI.SetActive(false);
    }
    IEnumerator FlashHeart(int index)
    {
        Image heart = heartImages[index];

        for (int i = 0; i < heartFlashCount; i++)
        {
            heart.enabled = false;
            yield return new WaitForSeconds(heartFlashDuration);

            heart.enabled = true;
            yield return new WaitForSeconds(heartFlashDuration);
        }

        heart.enabled = false;
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