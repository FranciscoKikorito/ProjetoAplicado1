using UnityEngine;

public class PunchVFXSpawner : MonoBehaviour
{
    [Header("Punch VFX Setup")]
    public GameObject punchVFXPrefab;
    public Transform handSpawnPoint; // This is the bone/Transform the VFX should follow

    // --- Animation Event Function ---
    public void SpawnPunchVFX()
    {
        if (punchVFXPrefab != null && handSpawnPoint != null)
        {
            // 1. Instantiate the VFX and store a reference.
            // We use Vector3.zero and Quaternion.identity because we want the 
            // VFX to be spawned at the parent's (handSpawnPoint's) local origin.
            GameObject tempVFX = Instantiate(
                punchVFXPrefab,
                handSpawnPoint.position, // Spawn at the hand's current world position
                handSpawnPoint.rotation  // Spawn with the hand's current world rotation
            );

            // 2. CRUCIAL FIX: Set the HandSpawnPoint as the parent of the VFX.
            tempVFX.transform.SetParent(handSpawnPoint, true);

            // NOTE: If you need to add an offset or rotation, you can now modify the 
            // tempVFX.transform.localPosition and tempVFX.transform.localRotation.
        }
    }
}