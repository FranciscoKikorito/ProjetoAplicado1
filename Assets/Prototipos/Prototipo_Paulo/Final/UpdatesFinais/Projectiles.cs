using UnityEngine;

public class Projectiles : MonoBehaviour
{
    public float speed = 20f;

    [Header("Efeitos Visuais")]
    public GameObject hitPlayerEffect; // vermelho
    public GameObject hitShieldEffect; // azul

    [Header("Efeitos Sonoros")]
    public AudioClip soundHitPlayer;   
    public AudioClip soundHitShield;   
    [Range(0f, 1f)] public float volume = 1f; 

    private Vector3 direction;
    private Transform targetToFollow; // O alvo para perseguir
    private bool isHoming = false;    // Se é teleguiado ou não

    // 1. Configuração para tiro reto (antigo)
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        isHoming = false;
        Destroy(gameObject, 5f); 
    }

    // 2. Configuração para tiro teleguiado (NOVO)
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
            
            // Opcional: Faz o projétil rodar fisicamente para olhar para o alvo
            transform.LookAt(targetToFollow);
        }

        // Move na direção (seja ela fixa ou atualizada pelo alvo)
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