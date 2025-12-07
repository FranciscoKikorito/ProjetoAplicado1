using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefab")]
    public GameObject tube;

    [Header("Tube Spawn Points (some may contain child points)")]
    public List<Transform> tubePossibleSpawnsList;

    void Start()
    {
        SpawnTubes();
    }

    private void SpawnTubes()
    {
        if (tube == null)
        {
            Debug.LogWarning("ObstacleSpawner: Tube prefab not assigned!");
            return;
        }

        if (tubePossibleSpawnsList.Count == 0)
        {
            Debug.LogWarning("ObstacleSpawner: No spawn points assigned!");
            return;
        }

        // Pick a random spawn point
        int randomIndex = Random.Range(0, tubePossibleSpawnsList.Count);
        Transform chosenPoint = tubePossibleSpawnsList[randomIndex];

        // Check if the chosen point has children
        if (chosenPoint.childCount > 0)
        {
            // Spawn on ALL children (paired spawn)
            for (int i = 0; i < chosenPoint.childCount; i++)
            {
                Transform child = chosenPoint.GetChild(i);
                SpawnTube(child);
            }
        }
        else
        {
            // Normal single-point spawn
            SpawnTube(chosenPoint);
        }
    }

    private void SpawnTube(Transform spawnPoint)
    {
        Instantiate(tube, spawnPoint.position, spawnPoint.rotation, spawnPoint);
    }
}
