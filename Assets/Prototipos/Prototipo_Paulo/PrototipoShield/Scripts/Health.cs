using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public int lives = 2;

    [Header("Flash Effect")]
    public float flashDuration = 0.15f;
    public int flashCount = 4;
    public Material flashMaterial; 
    public Transform playerModel;

    private Material[] originalMaterials;
    private Renderer[] renderers;

    [Header("Headband System")]
    public GameObject headbandOnHead;
    public GameObject droppedHeadbandPrefab;
    public Transform dropPoint;

    private bool firstHeadbandDropped = false;

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

        StartCoroutine(FlashEffect());

        if (!firstHeadbandDropped)
        {
            DropHeadband();
            firstHeadbandDropped = true;
        }

        if (lives <= 0)
            Die();
    }

    void DropHeadband()
    {
        if (headbandOnHead != null)
            headbandOnHead.SetActive(false);

        if (droppedHeadbandPrefab != null && dropPoint != null)
        {
            Instantiate(droppedHeadbandPrefab, dropPoint.position, dropPoint.rotation);
        }
    }
    
    public void RestoreHeadband()
    {
        if (headbandOnHead != null)
            headbandOnHead.SetActive(true);
        
        firstHeadbandDropped = false;
        lives = 2;
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

        // NOVO: dano por obstáculo (colisão física)
        if (other.collider.CompareTag("Obstacle"))
        {
            ApplyDamage(2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") && !other.CompareTag("Shield"))
        {
            ApplyDamage(1);
        }

        // NOVO: dano por obstáculo (trigger)
        if (other.CompareTag("Obstacle"))
        {
            ApplyDamage(2);
        }
    }
}