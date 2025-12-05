using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{

    [Header("Obstacles")]
    public GameObject tube;

    [Header("Tube Spawn Points")]
    public List<Transform> tubePossibleSpawnsList;

    void Start()
    {
        SpawnTubes();
    }

    private void SpawnTubes()
    {
        if (tubePossibleSpawnsList.Count == 0)
        {
            Debug.LogWarning("RoadSpawner: No road prefabs assigned!");
            return;
        }

        if (tubePossibleSpawnsList.Count == 0)
        {
            Debug.LogWarning("RoadSpawner: No spawn points assigned!");
            return;
        }

        /*
        foreach (Transform point in tubePossibleSpawnsList)
        {
            if (point == null) continue;

            SpawnRandomRoad(point);
        }
        */
        SpawnRandomTube(tubePossibleSpawnsList[0]);

    }

    private void SpawnRandomTube(Transform parentPoint)
    {
        //int index = Random.Range(0, roadPrefabs.Count);
        GameObject prefab = tube;

        Instantiate(prefab, parentPoint.position, parentPoint.rotation, parentPoint);
    }
}
