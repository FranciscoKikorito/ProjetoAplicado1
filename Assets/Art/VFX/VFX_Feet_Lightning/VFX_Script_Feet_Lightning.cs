// This script MUST be attached to your character's GameObject in the scene.
using UnityEngine;

public class FootstepVFXSpawner : MonoBehaviour
{
    public GameObject footstepVFXPrefab;
    public Transform leftFootSpawnPoint;
    public Transform rightFootSpawnPoint;

    public GameObject punchParticlePrefab;

    // --- NEW PUBLIC VARIABLES to control spawning ---
    public Vector3 spawnOffset = new Vector3(12f, -0.22f, 10f); // Adjust position
    public Vector3 spawnRotation = new Vector3(90f, 0f, 0f);    // Adjust rotation (e.g., 90 for flat)
    public Vector3 spawnScale = new Vector3(12f, 12f, 12f);  // Adjust size (e.g., 0.5 for half size)
    // ---------------------------------------------------

    public void SpawnLeftFootVFX()
    {
        GameObject temp = Instantiate(
            footstepVFXPrefab
        );        temp.transform.position = leftFootSpawnPoint.position + spawnOffset;
    }

    public void SpawnRightFootVFX()
    {

        GameObject temp = Instantiate(
            footstepVFXPrefab
        ); temp.transform.position = leftFootSpawnPoint.position + spawnOffset;
    }

    public void PunchParticle()
    {

    }
}