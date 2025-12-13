using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour
{
    [Header("Path Prefabs")]
    [SerializeField] private GameObject pathLineEmpty;
    [SerializeField] private GameObject pathLineObstacle;

    [SerializeField] private GameObject turnRightSection;
    [SerializeField] private GameObject turnLeftSection;

    [SerializeField] private GameObject furthestPlatform;

    [SerializeField] private int platformIntervalTurn;

    private Transform pathList;
    private int sectionCount;
    private float lastTilt = 0f;

    private int spawnSequenceIndex = 0;

    void Start()
    {
        pathList = GameObject.Find("PathList").transform;
        sectionCount = platformIntervalTurn - 1;

        ForceSpawnOneSection();
    }

    private void ForceSpawnOneSection()
    {
        SpawnNextSection();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Trigger")) return;
        SpawnNextSection();
    }

    private void SpawnNextSection()
    {
        sectionCount++;

        GameObject prefabToSpawn;
        bool isTurn = sectionCount % platformIntervalTurn == 0;
        bool nextIsTurn = (sectionCount + 1) % platformIntervalTurn == 0;

        Transform nextPoint = furthestPlatform.transform.Find("NextSectionPoint");
        Vector3 spawnPosition = nextPoint.position;
        Quaternion spawnRotation = nextPoint.rotation;

        if (isTurn)
        {
            prefabToSpawn = Random.value < 0.5f ? turnLeftSection : turnRightSection;
            Vector3 euler = spawnRotation.eulerAngles;
            euler.x = 0f;
            spawnRotation = Quaternion.Euler(euler);

            lastTilt = 0f;
            spawnSequenceIndex = 1;
        }
        else
        {
            if (spawnSequenceIndex == 0)
            {
                prefabToSpawn = pathLineEmpty;
            }
            else
            {
                prefabToSpawn = pathLineObstacle;
            }

            if (!nextIsTurn)
            {
                spawnSequenceIndex++;
                if (spawnSequenceIndex > 2)
                    spawnSequenceIndex = 0;
            }

            if (nextIsTurn)
            {
                Vector3 euler = spawnRotation.eulerAngles;
                euler.x = 0f;
                spawnRotation = Quaternion.Euler(euler);
                lastTilt = 0f;
            }
            else
            {
                float tiltToApply = 0f;

                if (Random.value < 0.3f)
                {
                    nextPoint.position += nextPoint.forward * -0.1f;
                    spawnPosition = nextPoint.position;

                    float proposedTilt = Random.value < 0.5f ? -15f : 15f;

                    if ((lastTilt == 15f && proposedTilt == -15f) ||
                        (lastTilt == -15f && proposedTilt == 15f))
                    {
                        tiltToApply = 0f;
                    }
                    else
                    {
                        tiltToApply = proposedTilt;
                    }
                }

                Vector3 euler = spawnRotation.eulerAngles;
                euler.x = tiltToApply;
                spawnRotation = Quaternion.Euler(euler);
                lastTilt = tiltToApply;
            }
        }

        GameObject newPlatform = Instantiate(prefabToSpawn, spawnPosition, spawnRotation, pathList);
        furthestPlatform = newPlatform;
    }
}
