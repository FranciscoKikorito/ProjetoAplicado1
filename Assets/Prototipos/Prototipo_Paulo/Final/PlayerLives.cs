using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerLives : MonoBehaviour
{
    public int lives = 3;

    [Header("UI Vidas & Efeitos")]
    public Image[] heartImages; 
    public GameObject livesUI; 
    public float uiVisibleAfterHitTime = 1.5f;
    public string runStateName = "Rig|Run";

    [Header("Efeito de Explosão do Coração")]
    public GameObject explosionParticlePrefab; // Arraste um Prefab de partículas aqui
    public Color damageColor = Color.red;      // Cor antes de explodir
    public float shakeIntensity = 5f;          // Força do tremor da UI
    
    [Header("Flash Effect (Player)")]
    public float flashDuration = 0.1f;
    public int flashCount = 5;
    public Material flashMaterial;
    public Transform playerModel;
    private Material[][] originalMaterials;
    private Renderer[] renderers;

    [Header("Animator")]
    public Animator animator;
    private bool isDead = false;

    [Header("Game Control")]
    public GameStartController gameController;

    [Header("Invencibilidade")]
    public float invincibilityDuration = 3.0f;
    public bool isInvincible = false;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip hitByLaserSFX;
    public AudioClip heartBreakSFX;

    private Vector3 originalUIPos; 
    void Awake()
    {
        if (playerModel == null) playerModel = transform;

        renderers = playerModel.GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].sharedMaterials;
        }

        if (animator == null) animator = GetComponent<Animator>();
        if (gameController == null) gameController = FindObjectOfType<GameStartController>();
        
        if (livesUI != null) 
        {
            originalUIPos = livesUI.transform.localPosition;
            livesUI.SetActive(false);
        }

        if (animator != null)
        {
            StartCoroutine(WaitForRunAndShowLives());
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
        
        StopCoroutine("HideLivesUIAfterDelay");
        
        StartCoroutine(LoseLifeSequence(previousLives));

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

    IEnumerator LoseLifeSequence(int previousLives)
    {
        int lostHeartIndex = previousLives - 1;
        
        StartCoroutine(ShakeUI(0.4f));

        if (lostHeartIndex >= 0 && lostHeartIndex < heartImages.Length)
        {
            Image heartLost = heartImages[lostHeartIndex];
            
            Color originalColor = heartLost.color;
            heartLost.color = damageColor;
            
            float timer = 0;
            while(timer < 0.2f)
            {
                heartLost.transform.localScale = Vector3.one * (1 + (timer * 2)); 
                timer += Time.deltaTime;
                yield return null;
            }

     
            if (explosionParticlePrefab != null)
            {
                GameObject explosion = Instantiate(explosionParticlePrefab, heartLost.transform.position, Quaternion.identity, livesUI.transform);
                Destroy(explosion, 2.0f);
            }
            
            if (audioSource && heartBreakSFX) audioSource.PlayOneShot(heartBreakSFX);
            
            heartLost.enabled = false;
            heartLost.transform.localScale = Vector3.one; 
            heartLost.color = originalColor;
        }
    }
    IEnumerator ShakeUI(float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            livesUI.transform.localPosition = originalUIPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }
        livesUI.transform.localPosition = originalUIPos;
    }
    void Die()
    {
        if (isDead) return;

        isDead = true;
        isInvincible = true;

        GameStartController.inputLocked = true;
        GameStartController.canJump = false;

        if (animator != null) animator.SetTrigger("Die");

        Debug.Log("Game Over!");
        if (gameController != null)
        {
            gameController.TriggerGameOver();
        }
    }
    IEnumerator WaitForRunAndShowLives()
    {
        yield return null; 
        
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(runStateName))
        {
            yield return null;
        }

        // Assim que entrar na animação de corrida, mostra a UI
        yield return StartCoroutine(ShowLivesAtStart());
    }

    IEnumerator ShowLivesAtStart()
    {
        livesUI.SetActive(true);
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = true;
            heartImages[i].transform.localScale = Vector3.zero;
        }

        // Animação de aparecer
        for (int i = 0; i < heartImages.Length; i++)
        {
            float t = 0;
            heartImages[i].enabled = true;
            while(t < 0.3f) {
                t += Time.deltaTime;
                heartImages[i].transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / 0.3f);
                yield return null;
            }
        }

        yield return new WaitForSeconds(2f);
        livesUI.SetActive(false);
    }

    IEnumerator HideLivesUIAfterDelay()
    {
        yield return new WaitForSeconds(uiVisibleAfterHitTime);
        livesUI.SetActive(false);
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
            for (int k = 0; k < matCount; k++) flashes[k] = flashMaterial;
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
        if (other.collider.CompareTag("Wall") && !other.collider.CompareTag("Shield")) ApplyDamage(1);
        if (other.collider.CompareTag("Obstacle")) ApplyDamage(1);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") && !other.CompareTag("Shield")) ApplyDamage(1);
        if (other.CompareTag("Obstacle")) ApplyDamage(1);
    }
}