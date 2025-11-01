using UnityEngine;

public class PlayerBridgeBuilder : MonoBehaviour
{
    private BridgeZone currentZone;
    private float holdTime = 0f;
    public float requiredHoldTime = 2f; // How long to hold space to build

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && currentZone != null && currentZone.CanBuild())
        {
            holdTime += Time.deltaTime;

            if (holdTime >= requiredHoldTime)
            {
                currentZone.BuildBridge();
                holdTime = 0f;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            holdTime = 0f; // Reset if player releases early
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        BridgeZone zone = other.GetComponent<BridgeZone>();
        if (zone != null)
            currentZone = zone;
    }

    private void OnTriggerExit(Collider other)
    {
        BridgeZone zone = other.GetComponent<BridgeZone>();
        if (zone != null && zone == currentZone)
            currentZone = null;
    }
}
