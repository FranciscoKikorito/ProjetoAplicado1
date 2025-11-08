using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 3f; // Distance within which enemy can be destroyed
    public KeyCode attackKey = KeyCode.Space;
    public LayerMask enemyLayer; // Assign layer for enemies
    public GameObject gameOverUI; // Optional UI for game over

    private void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        // Find all colliders within range on the enemy layer
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("Enemy destroyed!");
                Destroy(hit.gameObject);
                return; // Destroy only one per press
            }
        }

        Debug.Log("No enemy in range!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Game Over!");
            if (gameOverUI != null)
                gameOverUI.SetActive(true); // Show game over UI if assigned
            else
                Time.timeScale = 0; // Pause the game as a fallback
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw attack radius in the editor for clarity
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
