using UnityEngine;

public class BridgeZone : MonoBehaviour
{
    public GameObject bridgePrefab;
    private bool playerInside = false;
    private bool isBuilt = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    public bool CanBuild()
    {
        return playerInside && !isBuilt;
    }

    public void BuildBridge()
    {
        if (isBuilt) return;

        Instantiate(bridgePrefab, transform.position, transform.rotation);
        isBuilt = true;
    }
}
