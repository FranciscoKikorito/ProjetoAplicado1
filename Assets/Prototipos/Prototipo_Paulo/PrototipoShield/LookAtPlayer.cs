using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LookAtPlayer : MonoBehaviour
{
    public Transform lookAtTarget;
    private Quaternion rotate;

    public float rotationDamp = 2.0f;
    public float range = 50.0f;

    private RaycastHit hit;
    private LineRenderer laserLine;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            lookAtTarget = player.transform;

        // Configura o LineRenderer
        laserLine = GetComponent<LineRenderer>();
        laserLine.enabled = false; // começa invisível
        laserLine.positionCount = 2;
        laserLine.startWidth = 0.05f;
        laserLine.endWidth = 0.05f;

        // Cor opcional do laser
        laserLine.startColor = Color.red;
        laserLine.endColor = Color.red;
    }

    void Update()
    {
        if (lookAtTarget == null)
            return;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        float distanceToPlayer = Vector3.Distance(lookAtTarget.position, transform.position);

        if (distanceToPlayer <= range)
        {
            rotate = Quaternion.LookRotation(lookAtTarget.position - transform.position);

            if (Physics.Raycast(transform.position, forward, out hit, range))
            {
                // Mostra o laser do inimigo até o ponto atingido
                laserLine.enabled = true;
                laserLine.SetPosition(0, transform.position);
                laserLine.SetPosition(1, hit.point);

                if (hit.collider.CompareTag("Player"))
                {
                    Shoot();
                }
            }
            else
            {
                // Caso não atinja nada, estende o laser até o limite do alcance
                laserLine.enabled = true;
                laserLine.SetPosition(0, transform.position);
                laserLine.SetPosition(1, transform.position + forward * range);
            }
        }
        else
        {
            rotate = Quaternion.identity;
            laserLine.enabled = false;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * rotationDamp);
    }

    void Shoot()
    {
        Debug.Log("Shoot at player at " + Time.time);
    }
}
