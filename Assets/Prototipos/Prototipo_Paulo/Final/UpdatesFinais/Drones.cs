using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
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
    private Transform pathCenter; 
    public float moveSpeed = 5f;
    public AudioClip detectionSound;
    
    private Transform aimPoint;
    private AudioSource audioSource;
    private bool hasShot = false;
    private bool isWarning = false;

    void Start()
    {
        // Busca automática do TargetPathCenter
        GameObject foundTarget = GameObject.Find("TargetPathCenter");
        if (foundTarget != null) pathCenter = foundTarget.transform;
        else Debug.LogError("ERRO: 'TargetPathCenter' não encontrado.");

        // Busca automática do AimPoint (O alvo do tiro)
        GameObject target = GameObject.Find("AimPoint");
        if (target != null) aimPoint = target.transform;
        else Debug.LogError("AimPoint não encontrado na cena!");

        audioSource = GetComponent<AudioSource>();
        if(warningParticles != null) warningParticles.Stop();
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
        if (warningParticles != null) warningParticles.Play();
        if (detectionSound != null && audioSource != null) audioSource.PlayOneShot(detectionSound);

        float timer = 0f;
        while (timer < warningDuration)
        {
            timer += Time.deltaTime;
            if (pathCenter != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, pathCenter.position, moveSpeed * Time.deltaTime);
                transform.LookAt(aimPoint); 
            }
            yield return null;
        }

        if (warningParticles != null) warningParticles.Stop();
        
        Shoot(); // Dispara
        hasShot = true;
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || aimPoint == null) return;

        // 1. Cria o projétil no bico do drone
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        // 2. Passa o AimPoint como ALVO para o projétil perseguir
        Projectiles projScript = projectile.GetComponent<Projectiles>();
        if (projScript != null)
        {
            // MUDANÇA AQUI: Usamos SetTarget em vez de SetDirection
            projScript.SetTarget(aimPoint);
        }
    }
}