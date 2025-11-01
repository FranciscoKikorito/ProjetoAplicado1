using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 3f;
    public LayerMask enemyLayer;
    public GameObject gameOverUI;
    public float tapThreshold = 0.2f; // Seconds to count as a quick tap

    [Header("Bridge Settings")]
    public float holdToBuildTime = 2f;
    private BridgeZone currentZone;

    private float spaceHoldTime = 0f;
    private bool isHolding = false;

    void Update()
    {
        // Space pressed down
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spaceHoldTime = 0f;
            isHolding = true;
        }

        // Space held
        if (isHolding && Input.GetKey(KeyCode.Space))
        {
            spaceHoldTime += Time.deltaTime;

            // If we are inside a bridge zone and held long enough -> build bridge
            if (currentZone != null && currentZone.CanBuild() && spaceHoldTime >= holdToBuildTime)
            {
                currentZone.BuildBridge();
                isHolding = false; // stop counting
            }
        }

        // Space released
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (spaceHoldTime < tapThreshold)
            {
                TryAttack(); // short press = attack
            }

            isHolding = false;
            spaceHoldTime = 0f;
        }
    }

    void TryAttack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("Enemy destroyed!");
                Destroy(hit.gameObject);
                return;
            }
        }

        Debug.Log("No enemy in range!");
    }

    private void OnTriggerEnter(Collider other)
    {
        BridgeZone zone = other.GetComponent<BridgeZone>();
        if (zone != null)
            currentZone = zone;

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Game Over!");
            if (gameOverUI != null)
                gameOverUI.SetActive(true);
            else
                Time.timeScale = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BridgeZone zone = other.GetComponent<BridgeZone>();
        if (zone != null && zone == currentZone)
            currentZone = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
