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
    
    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip hitByLaserSFX;
    void Awake()
    {
        renderers = playerModel.GetComponentsInChildren<Renderer>();
        
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalMaterials[i] = renderers[i].material;
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
        Debug.Log("Player morreu!");
        gameObject.SetActive(false);
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