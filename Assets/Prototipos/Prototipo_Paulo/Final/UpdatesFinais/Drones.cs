using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))] // Garante que o objeto tem um AudioSource
public class Drones : MonoBehaviour
{
    [Header("Ataque")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float detectionRange = 20f;

    [Header("Efeitos")]
    public ParticleSystem warningParticles;
    public float warningDuration = 1.2f;
    
    [Header("Movimento e Som")]
    public Transform pathCenter; // Arraste o objeto "Pai" ou o centro da rota aqui
    public float moveSpeed = 5f; // Velocidade de retorno ao centro
    public AudioClip detectionSound; // O som robótico
    
    private Transform aimPoint;
    private AudioSource audioSource;
    private bool hasShot = false;
    private bool isWarning = false;

    void Start()
    {
        // Tenta encontrar o player/alvo
        GameObject target = GameObject.Find("AimPoint");
        if (target != null)
        {
            aimPoint = target.transform;
        }
        else
        {
            Debug.LogError("AimPoint não encontrado na cena!");
        }

        audioSource = GetComponent<AudioSource>();
        warningParticles.Stop();
    }

    void Update()
    {
        if (aimPoint == null || hasShot || isWarning) return;

        float distance = Vector3.Distance(transform.position, aimPoint.position);

        if (distance <= detectionRange)
        {
            StartCoroutine(WarningAndShoot());
        }
    }

    IEnumerator WarningAndShoot()
    {
        isWarning = true;

        // 1. Começa as partículas
        warningParticles.Play();

        // 2. Toca o som (se houver)
        if (detectionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(detectionSound);
        }

        // 3. Move para o centro ENQUANTO espera o tempo do aviso
        float timer = 0f;
        while (timer < warningDuration)
        {
            timer += Time.deltaTime;

            // Lógica de Movimento
            if (pathCenter != null)
            {
                // Move o drone em direção ao centro da pathlist
                transform.position = Vector3.MoveTowards(transform.position, pathCenter.position, moveSpeed * Time.deltaTime);
                
                // Opcional: Faz o drone olhar para o player enquanto recua (dá um efeito mais ameaçador)
                transform.LookAt(aimPoint); 
            }

            yield return null; // Espera o próximo frame
        }

        warningParticles.Stop();

        Shoot();
        hasShot = true;
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        Vector3 direction = (aimPoint.position - firePoint.position).normalized;
        
        // Verifica se o projétil é do tipo Projectiles ou LightningProjectile e ajusta
        // Assumindo que usa o script Projectiles anterior:
        Projectiles projScript = projectile.GetComponent<Projectiles>();
        if (projScript != null)
        {
            projScript.SetDirection(direction);
        }
    }
}