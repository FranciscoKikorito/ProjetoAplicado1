using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerLives1 : MonoBehaviour
{
    public int lives = 2;

    [Header("Flash Effect")]
    public float flashDuration = 0.1f;
    public int flashCount = 6;
    public Material flashMaterial;
    public Transform playerModel;
    private Material[] originalMaterials;
    private Renderer[] renderers;

    [Header("Animator")]
    public Animator animator;

    [Header("Invencibilidade")]
    public float invincibleTime = 1.5f;
    private bool isInvincible = false;

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
        if (isInvincible)
            return;

        lives -= amount;
        Debug.Log("Player levou dano. Vidas: " + lives);

        if (audioSource && hitByLaserSFX)
            audioSource.PlayOneShot(hitByLaserSFX);

        StartCoroutine(Invincibility());
        StartCoroutine(FlashEffect());

        if (lives <= 0)
            GameOver();
    }

    IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
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

    void GameOver()
    {
        Debug.Log("GAME OVER!");
        animator.SetTrigger("Die");

        StartCoroutine(RestartAfterDelay());
    }
    
    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    } 
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Obstacle"))
        {
            ApplyDamage(2);
        }
        //if (other.collider.CompareTag("Wall") && !other.collider.CompareTag("Shield"))
        //{
        //    ApplyDamage(1);
        //}
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            ApplyDamage(2);
        }
        //if (other.CompareTag("Wall") && !other.CompareTag("Shield"))
        //{
        //    ApplyDamage(1);
        //}
    }
}