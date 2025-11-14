using System.Collections.Generic;
using UnityEngine;

public class ProceduralHouseSpawner : MonoBehaviour
{
    [Header("Prefabs To Spawn")]
    public List<GameObject> spawnPrefabs;

    [Header("Procedural Points")]
    public List<Transform> leftPointsList;
    public List<Transform> rightPointsList;

    void Start()
    {
        SpawnAtPoints(leftPointsList, 180f);
        SpawnAtPoints(rightPointsList, 0f);
    }

    void SpawnAtPoints(List<Transform> points, float yRotationOffset)
    {
        if (spawnPrefabs.Count == 0) return;

        foreach (Transform point in points)
        {
            GameObject randomPrefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Count)];
            Quaternion finalRotation = point.rotation * Quaternion.Euler(0, yRotationOffset, 0);
            GameObject instance = Instantiate(randomPrefab, point.position, finalRotation, point);

            float randomScaleY = Random.Range(0.8f, 1.2f);
            Vector3 originalScale = instance.transform.localScale;
            instance.transform.localScale = new Vector3(
                originalScale.x,
                randomScaleY,
                originalScale.z
            );
        }
    }
}
