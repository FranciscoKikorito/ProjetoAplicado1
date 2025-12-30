using UnityEngine;

public class Projectiles : MonoBehaviour
{
    public float speed = 20f;

    [Header("Efeitos Visuais")]
    public GameObject hitPlayerEffect; 
    public GameObject hitShieldEffect; 
    
    [Header("Efeitos Sonoros")]
    public AudioClip soundHitPlayer;   
    public AudioClip soundHitShield;   
    [Range(0f, 1f)] public float volume = 1f; 

    private Vector3 direction;
    private Transform targetToFollow; 
    private bool isHoming = false;    
    public void SetTarget(Transform target)
    {
        targetToFollow = target;
        isHoming = true;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Se for teleguiado e o alvo ainda existir, recalcula a direção
        if (isHoming && targetToFollow != null)
        {
            // Calcula a direção atualizada para o centro do alvo
            direction = (targetToFollow.position - transform.position).normalized;
            
            transform.LookAt(targetToFollow);
        }
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // SHIELD
        if (other.CompareTag("Shield"))
        {
            if (soundHitShield != null) AudioSource.PlayClipAtPoint(soundHitShield, transform.position, volume);
            SpawnEffect(hitShieldEffect, other.transform);
            Destroy(gameObject);
            return;
        }

        // PLAYER
        if (other.CompareTag("PlayerFront"))
        {
            PlayerLives health = other.GetComponentInParent<PlayerLives>();
            if (health != null) health.ApplyDamage(1);

            if (soundHitPlayer != null) AudioSource.PlayClipAtPoint(soundHitPlayer, transform.position, volume);
            SpawnEffect(hitPlayerEffect, other.transform);
            Destroy(gameObject);
        }
    }

    void SpawnEffect(GameObject effect, Transform parent)
    {
        if (effect != null)
        {
            Instantiate(effect, transform.position - direction * 0.1f, Quaternion.identity, parent);
        }
    }
}