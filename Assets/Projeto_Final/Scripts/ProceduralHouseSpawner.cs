using System.Collections.Generic;
using UnityEngine;

public class ProceduralHouseSpawner : MonoBehaviour
{
    [Header("Prefabs 1x1 (uses 1 slot)")]
    public List<GameObject> spawnPrefabs1x1;

    [Header("Prefabs 1x2 (uses 2 consecutive slots)")]
    public List<GameObject> spawnPrefabs1x2;

    [Header("Prefabs 1x3 (uses 3 consecutive slots)")]
    public List<GameObject> spawnPrefabs1x3;

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
        int i = 0;

        while (i < points.Count)
        {
            
            int size = PickPrefabSize();
            if (size == 3 && i > points.Count - 3) size = 1;
            if (size == 2 && i > points.Count - 2) size = 1;

            GameObject prefab = GetRandomPrefabBySize(size);

            if (prefab == null)
            {
                prefab = GetRandomPrefabBySize(1);
                size = 1;
            }

            Transform point = points[i];
            Quaternion finalRot = point.rotation * Quaternion.Euler(0, yRotationOffset, 0);

            GameObject instance = Instantiate(prefab, point.position, finalRot, point);

            // random scale Y
            float randomY = Random.Range(0.8f, 1.2f);
            Vector3 originalScale = instance.transform.localScale;
            instance.transform.localScale = new Vector3(originalScale.x, randomY, originalScale.z);

            i += size;
        }
    }

    int PickPrefabSize()
    {
        int roll = Random.Range(0, 100);

        if (roll < 33) return 1;
        if (roll < 66) return 2;
        return 3;
    }

    GameObject GetRandomPrefabBySize(int size)
    {
        switch (size)
        {
            case 1:
                if (spawnPrefabs1x1.Count > 0)
                    return spawnPrefabs1x1[Random.Range(0, spawnPrefabs1x1.Count)];
                break;

            case 2:
                if (spawnPrefabs1x2.Count > 0)
                    return spawnPrefabs1x2[Random.Range(0, spawnPrefabs1x2.Count)];
                break;

            case 3:
                if (spawnPrefabs1x3.Count > 0)
                    return spawnPrefabs1x3[Random.Range(0, spawnPrefabs1x3.Count)];
                break;
        }
        return null;
    }
}
