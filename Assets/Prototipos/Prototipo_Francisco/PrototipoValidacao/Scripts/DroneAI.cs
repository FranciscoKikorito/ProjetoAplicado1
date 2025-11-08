using UnityEngine;
using System.Collections;

public class DroneAI : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public Transform firePoint;
    public ParticleSystem chargeParticles;
    public GameObject laserImpactFX;

    [Header("Configurações de ataque")]
    public float detectionRange = 20f;
    public float laserDuration = 3f;     // 👈 laser visível por 3 segundos
    public float chargeTime = 1.5f;      // tempo de carregamento antes de atirar

    private bool hasDetected = false;
    private bool hasShot = false;
    private LineRenderer lineRenderer;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Garante que temos um LineRenderer configurado
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        var mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = Color.red;
        lineRenderer.material = mat;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        if (player == null || firePoint == null || hasShot) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Detecta apenas quando o player ENTRA pela primeira vez
        if (!hasDetected && distance <= detectionRange)
        {
            hasDetected = true;
            StartCoroutine(PrepareAndShoot());
        }
    }

    IEnumerator PrepareAndShoot()
    {
        // 🔋 Inicia partículas de carregamento
        if (chargeParticles != null && !chargeParticles.isPlaying)
            chargeParticles.Play();

        // Espera o tempo de carregamento
        yield return new WaitForSeconds(chargeTime);

        // Para o carregamento visual
        if (chargeParticles != null)
            chargeParticles.Stop();

        // Direção do laser
        Vector3 playerCenter = player.position + Vector3.up * 1.2f;
        Vector3 direction = (playerCenter - firePoint.position).normalized;

        // Dispara o laser
        ShootLaser(direction);
        hasShot = true;
    }

    void ShootLaser(Vector3 direction)
    {
        int mask = LayerMask.GetMask("Default", "Player", "Shield");
        RaycastHit hit;

        Vector3 endPoint = firePoint.position + direction * detectionRange;

        if (Physics.Raycast(firePoint.position, direction, out hit, detectionRange, mask))
        {
            endPoint = hit.point;

            // Impacto visual
            if (laserImpactFX != null)
            {
                GameObject fx = Instantiate(laserImpactFX, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(fx, 2f);
            }

            // Se o escudo interceptar
            var shield = hit.collider.GetComponentInParent<PlayerShield>();
            if (shield != null && shield.IsShieldActive())
            {
                Debug.Log("🛡️ Laser bloqueado no escudo!");
            }
            else
            {
                Debug.Log("💥 Laser atingiu o jogador ou objeto!");
            }
        }

        // Mostra o laser por 3 segundos
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, endPoint);
        StartCoroutine(DisableLaserAfter(laserDuration));
    }

    IEnumerator DisableLaserAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        lineRenderer.enabled = false;
        Debug.Log("🔻 Laser desativado.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}