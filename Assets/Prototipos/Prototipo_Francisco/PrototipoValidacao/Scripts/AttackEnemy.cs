using UnityEngine;

public class AttackEnemy : MonoBehaviour
{
    [SerializeField] private LayerMask enemyTriggerLayer;

    private Collider currentTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyTriggerLayer) != 0)
        {
            currentTrigger = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == currentTrigger)
        {
            currentTrigger = null;
        }
    }

    private void Update()
    {
        if (currentTrigger != null && Input.GetMouseButtonDown(0)) // 0 = left click
        {
            if (currentTrigger.transform.parent != null)
            {
                Destroy(currentTrigger.transform.parent.gameObject);
                currentTrigger = null;
            }
        }
    }
}
