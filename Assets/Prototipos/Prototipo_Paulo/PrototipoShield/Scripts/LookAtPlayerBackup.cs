
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LookAtPlayerBackup : MonoBehaviour
{
    public Transform lookAtTarget;
    public Transform firePoint;
    public Transform aimPoint;
    public GameObject hitEffectPrefab;

    [Header("Configurações")]
    public float rotationSpeed = 180f;
    public float maxRotationAngle = 45f;
    public float detectionRange = 40f;
    public float laserRange = 17f;
    public float aimDelay = 1.5f;  
    public float maxLaserDuration = 1.2f;
    public ParticleSystem chargingParticles;
    
    private LineRenderer laser;
    private bool isAiming = false;
    private bool hasShot = false;
    private float aimTimer = 0f;
    private float laserTimer = 0f;
    void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("PlayerFront");
        if (player != null)
        {
            lookAtTarget = player.transform;
            aimPoint = player.transform.Find("AimPoint");
        }

        laser = GetComponent<LineRenderer>();
        laser.startColor = Color.orangeRed;
        laser.enabled = false;

        if (firePoint == null)
            firePoint = transform;
    }

    void Update()
    {
        if (hasShot || lookAtTarget == null)
        {
            laser.enabled = false;
            return;
        }

        Vector3 dir = (aimPoint.position - transform.position);
        float dist = dir.magnitude;
        Vector3 flatDir = new Vector3(dir.x, 0, dir.z);
        float angle = Vector3.Angle(transform.forward, flatDir);

        if (dist > detectionRange || angle > maxRotationAngle)
        {
            StopCharge();
            return;
        }
        
        Quaternion targetRot = Quaternion.LookRotation(flatDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        
        if (!isAiming)
        {
            isAiming = true;
            aimTimer = 0f;

            if (chargingParticles != null)
                chargingParticles.Play();
        }
        
        if (aimTimer < aimDelay)
        {
            aimTimer += Time.deltaTime;
            return;
        }
        
        StopCharge();
        
        if (laserTimer < maxLaserDuration)
        {
            laserTimer += Time.deltaTime;
            FireLaser();
        }
        else
        {
            ShootOnce();
        }
    }

    void StopCharge()
    {
        if (chargingParticles != null)
            chargingParticles.Stop();
    }

    void FireLaser()
    {
        laser.enabled = true;

        Vector3 dir = (aimPoint.position - firePoint.position).normalized;

        if (Physics.Raycast(firePoint.position, dir, out RaycastHit hit, laserRange))
        {
            
            GameObject fx = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(fx, 1f);
            
            laser.SetPosition(0, firePoint.position);
            laser.SetPosition(1, hit.point);
            
            if (hit.collider.CompareTag("PlayerFront"))
                ShootOnce();
        }
        else
        {
            laser.SetPosition(0, firePoint.position);
            laser.SetPosition(1, firePoint.position + dir * laserRange);
        }
    }
    void ShootOnce()
    {
        if (hasShot) return;
        hasShot = true;
        laser.enabled = false;
    }
}
