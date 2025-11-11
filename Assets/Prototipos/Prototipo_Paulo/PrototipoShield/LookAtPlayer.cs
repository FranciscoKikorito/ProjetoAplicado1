using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LookAtPlayer : MonoBehaviour
{
    public Transform lookAtTarget;
    public Transform firePoint;
    private Quaternion rotate;

    [Header("Configurações de Rotação")]
    public float rotationDamp = 5.0f;
    public float maxRotationAngle = 45f;

    [Header("Distâncias")]
    public float detectionRange = 25.0f;
    public float laserRange = 40.0f;

    [Header("Delay e Partículas")]
    public float aimDelay = 2f; // Tempo de preparação antes de atirar
    public ParticleSystem chargingParticles; // Sistema de partículas de aviso

    private RaycastHit hit;
    private LineRenderer laserLine;
    private bool canShoot = false;
    private bool isAiming = false;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            lookAtTarget = player.transform;

        laserLine = GetComponent<LineRenderer>();
        laserLine.enabled = false;
        laserLine.positionCount = 2;
        laserLine.startWidth = 0.05f;
        laserLine.endWidth = 0.05f;
        laserLine.startColor = Color.red;
        laserLine.endColor = Color.red;

        if (firePoint == null)
            firePoint = transform;
    }

    void Update()
    {
        if (lookAtTarget == null)
            return;

        Vector3 forward = transform.forward;
        Vector3 toPlayer = lookAtTarget.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        Vector3 flatToPlayer = new Vector3(toPlayer.x, 0, toPlayer.z);
        if (flatToPlayer.sqrMagnitude < 0.01f)
            return;

        float angle = Vector3.Angle(forward, flatToPlayer);

        if (distanceToPlayer <= detectionRange && angle <= maxRotationAngle)
        {
            // Rotação horizontal suave
            Quaternion targetRotation = Quaternion.LookRotation(flatToPlayer.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationDamp);

            // Inicia a corrotina de aiming se ainda não começou
            if (!isAiming)
            {
                StartCoroutine(AimBeforeShooting());
            }

            // 🔴 Só dispara quando o aiming terminar
            if (canShoot)
            {
                FireLaser();
            }
        }
        else
        {
            StopAllCoroutines();
            isAiming = false;
            canShoot = false;
            if (chargingParticles != null)
                chargingParticles.Stop();
            laserLine.enabled = false;
        }
    }

    IEnumerator AimBeforeShooting()
    {
        isAiming = true;

        // Ativa partículas de aviso
        if (chargingParticles != null)
            chargingParticles.Play();

        // Espera o delay
        yield return new WaitForSeconds(aimDelay);

        // Pode atirar
        canShoot = true;

        // Para partículas quando o laser dispara
        if (chargingParticles != null)
            chargingParticles.Stop();
    }

    void FireLaser()
    {
        Vector3 forwardDir = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(firePoint.position, forwardDir, out hit, laserRange))
        {
            laserLine.enabled = true;
            laserLine.SetPosition(0, firePoint.position);
            laserLine.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("Player"))
            {
                Shoot();
            }
        }
        else
        {
            // Caso não acerte nada, desce até o chão
            RaycastHit groundHit;
            Vector3 downStart = firePoint.position + forwardDir * laserRange;
            if (Physics.Raycast(downStart, Vector3.down, out groundHit, 100f))
            {
                laserLine.enabled = true;
                laserLine.SetPosition(0, firePoint.position);
                laserLine.SetPosition(1, groundHit.point);
            }
            else
            {
                laserLine.enabled = true;
                laserLine.SetPosition(0, firePoint.position);
                laserLine.SetPosition(1, firePoint.position + forwardDir * laserRange);
            }
        }
    }

    void Shoot()
    {
        Debug.Log("Shoot at player at " + Time.time);
    }
}