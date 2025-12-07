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

    // Track last platform's X rotation tilt (-15, 0, 15)
    private float lastTilt = 0f;

    // Tracks the alternating spawn logic
    private int spawnSequenceIndex = 0; // 0 = empty, 1 & 2 = obstacles

    void Start()
    {
        pathList = GameObject.Find("PathList").transform;
        sectionCount = platformIntervalTurn - 1;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Trigger")) return;

        sectionCount++;

        GameObject prefabToSpawn;
        bool isTurn = sectionCount % platformIntervalTurn == 0;
        bool nextIsTurn = (sectionCount + 1) % platformIntervalTurn == 0;

        // Determine spawn position
        Transform nextPoint = furthestPlatform.transform.Find("NextSectionPoint");
        Vector3 spawnPosition = nextPoint.position;
        Quaternion spawnRotation = nextPoint.rotation;

        if (isTurn)
        {
            // Turn piece: always X = 0
            prefabToSpawn = Random.value < 0.5f ? turnLeftSection : turnRightSection;
            Vector3 euler = spawnRotation.eulerAngles;
            euler.x = 0f;
            spawnRotation = Quaternion.Euler(euler);

            lastTilt = 0f; // reset last tilt

            // Force next path to be an obstacle
            spawnSequenceIndex = 1;
        }
        else
        {
            // Normal path piece: use alternating logic
            if (spawnSequenceIndex == 0)
            {
                prefabToSpawn = pathLineEmpty;
            }
            else
            {
                prefabToSpawn = pathLineObstacle;
            }

            // Update sequence index for next spawn (skip if we just set it from a turn)
            if (!nextIsTurn) 
            {
                spawnSequenceIndex++;
                if (spawnSequenceIndex > 2) // after two obstacles, reset to empty
                    spawnSequenceIndex = 0;
            }

            // Tilt logic
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

        // Instantiate platform
        GameObject newPlatform = Instantiate(prefabToSpawn, spawnPosition, spawnRotation, pathList);
        furthestPlatform = newPlatform;
    }
}
