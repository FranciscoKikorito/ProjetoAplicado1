using UnityEngine;

using UnityEngine;

public class Projectiles : MonoBehaviour
{
    public float speed = 20f;

    [Header("Efeitos Visuais")]
    public GameObject hitPlayerEffect; // vermelho
    public GameObject hitShieldEffect; // azul

    [Header("Efeitos Sonoros")]
    public AudioClip soundHitPlayer;   // Som de dano/carne
    public AudioClip soundHitShield;   // Som metálico/energia
    [Range(0f, 1f)] public float volume = 1f; // Controle de volume

    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, 5f); // Destroi após 5s se não bater em nada
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // SHIELD
        if (other.CompareTag("Shield"))
        {
            Debug.Log("ACERTOU NO SHIELD");

            // Toca o som no local do impacto antes de destruir o objeto
            if (soundHitShield != null)
            {
                AudioSource.PlayClipAtPoint(soundHitShield, transform.position, volume);
            }

            SpawnEffect(hitShieldEffect);
            Destroy(gameObject);
            return;
        }

        // PLAYER
        if (other.CompareTag("PlayerFront"))
        {
            PlayerLives health = other.GetComponentInParent<PlayerLives>();
            if (health != null)
            {
                health.ApplyDamage(1);
            }

            // Toca o som no local do impacto antes de destruir o objeto
            if (soundHitPlayer != null)
            {
                AudioSource.PlayClipAtPoint(soundHitPlayer, transform.position, volume);
            }

            SpawnEffect(hitPlayerEffect);
            Destroy(gameObject);
        }
    }

    void SpawnEffect(GameObject effect)
    {
        if (effect != null)
        {
            Instantiate(
                effect,
                transform.position - direction * 0.1f, // Recua um pouco para não ficar dentro do objeto
                Quaternion.identity
            );
        }
    }
}
