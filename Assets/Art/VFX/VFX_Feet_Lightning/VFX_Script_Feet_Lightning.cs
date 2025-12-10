using UnityEngine;
// ... other using statements

public class FootstepVFXSpawner : MonoBehaviour
{
    public GameObject footstepVFXPrefab;
    public Transform leftFootSpawnPoint;
    public Transform rightFootSpawnPoint;

    // Add a debug line to confirm this function is called
    public void SpawnLeftFootVFX()
    {
        Debug.Log("Left Foot VFX function called!"); // <-- Check console for this!

        // Your VFX instantiation logic for the left foot
        if (footstepVFXPrefab != null && leftFootSpawnPoint != null)
        {
            Instantiate(footstepVFXPrefab, leftFootSpawnPoint.position, leftFootSpawnPoint.rotation);
        }
    }

    public void SpawnRightFootVFX()
    {
        Debug.Log("Right Foot VFX function called!"); // <-- Check console for this!

        // Your VFX instantiation logic for the right foot
        if (footstepVFXPrefab != null && rightFootSpawnPoint != null)
        {
            Instantiate(footstepVFXPrefab, rightFootSpawnPoint.position, rightFootSpawnPoint.rotation);
        }
    }
}