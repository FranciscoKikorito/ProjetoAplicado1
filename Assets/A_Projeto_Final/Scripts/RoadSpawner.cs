using UnityEngine;
using System.Collections.Generic;

public class RoadSpawner : MonoBehaviour
{
    [Header("Spawn Points (Transforms)")]
    [SerializeField] public List<Transform> roadPointsList;

    [Header("Road Prefabs")]
    [SerializeField] private List<GameObject> roadPrefabs = new List<GameObject>();

    private void Start()
    {
        SpawnRoads();
    }

    private void SpawnRoads()
    {
        if (roadPrefabs.Count == 0)
        {
            Debug.LogWarning("RoadSpawner: No road prefabs assigned!");
            return;
        }

        if (roadPointsList.Count == 0)
        {
            Debug.LogWarning("RoadSpawner: No spawn points assigned!");
            return;
        }

        foreach (Transform point in roadPointsList)
        {
            if (point == null) continue;

            SpawnRandomRoad(point);
        }
    }

    private void SpawnRandomRoad(Transform parentPoint)
    {
        int index = Random.Range(0, roadPrefabs.Count);
        GameObject prefab = roadPrefabs[index];

        Instantiate(prefab, parentPoint.position, parentPoint.rotation, parentPoint);
    }
}
