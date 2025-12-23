using UnityEngine;
using System.Collections;
public class Drones : MonoBehaviour
{
    [Header("Ataque")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float detectionRange = 20f;

    [Header("Efeitos Visuais")]
    public ParticleSystem warningParticles;
    public float warningDuration = 1.2f;

    [Header("--- ÁUDIO & VOLUMES ---")]
    [Header("1. Som de Carregar (Charging)")]
    public AudioClip chargingSound;
    [Range(0f, 1f)] public float chargingVolume = 0.5f;

    [Header("2. Som de Tiro (Shooting)")]
    public AudioClip[] shootSounds;
    [Range(0f, 1f)] public float shootingVolume = 1.0f;
    
    [Header("Movimento")]
    private Transform pathCenter;
    public float moveSpeed = 5f;

    private Transform aimPoint;
    
    // TRÊS AUDIOSOURCES DISTINTOS
    private AudioSource chargingSource;
    private AudioSource shootingSource;

    private bool hasShot = false;
    private bool isWarning = false;

    void Start()
    {
        // Cria os componentes no objeto automaticamente ao iniciar o jogo
        chargingSource = gameObject.AddComponent<AudioSource>();
        shootingSource = gameObject.AddComponent<AudioSource>();

        // Configura volumes iniciais
        chargingSource.volume = chargingVolume;
        shootingSource.volume = shootingVolume;
        
        // Configurações extra para 3D (para o som diminuir com a distância)
        ConfigureAudio3D(chargingSource);
        ConfigureAudio3D(shootingSource);
        // ----------------------------------------------------

        // Busca TargetPathCenter e AimPoint
        GameObject foundTarget = GameObject.Find("TargetPathCenter");
        if (foundTarget != null) pathCenter = foundTarget.transform;
        else Debug.LogError("ERRO: 'TargetPathCenter' não encontrado.");

        GameObject target = GameObject.Find("AimPoint");
        if (target != null) aimPoint = target.transform;
        else Debug.LogError("AimPoint não encontrado!");

        if (warningParticles != null) warningParticles.Stop();
    }

    // Função auxiliar para configurar som 3D rapidamente
    void ConfigureAudio3D(AudioSource source)
    {
        source.spatialBlend = 1f; // Torna o som 3D
        source.minDistance = 2f;
        source.maxDistance = detectionRange * 1.5f;
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

        // USANDO O AUDIO SOURCE 1: CHARGING
        if (chargingSound != null && chargingSource != null)
        {
            chargingSource.clip = chargingSound;
            chargingSource.volume = chargingVolume; // Garante o volume certo
            chargingSource.Play();
        }

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
        
        // Para o som de charging
        if (chargingSource != null) chargingSource.Stop();

        Shoot();
        hasShot = true;
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || aimPoint == null) return;
        
        if (shootSounds != null && shootSounds.Length > 0 && shootingSource != null)
        {
            int randomIndex = Random.Range(0, shootSounds.Length);
            shootingSource.PlayOneShot(shootSounds[randomIndex], shootingVolume);
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        Projectiles projScript = projectile.GetComponent<Projectiles>();
        if (projScript != null)
        {
            projScript.SetTarget(aimPoint);
        }
    }
}