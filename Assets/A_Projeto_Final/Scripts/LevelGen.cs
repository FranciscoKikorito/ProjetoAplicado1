using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour
{
    [SerializeField] public GameObject[] pathSections;
    [SerializeField] private GameObject turnRightSection;
    [SerializeField] private GameObject turnLeftSection;

    [SerializeField] private GameObject furthestPlatform;

    [SerializeField] private int platformIntervalTurn;

    private Transform pathList;
    private int sectionCount;

    // Track last platform's X rotation tilt (-15, 0, 15)
    private float lastTilt = 0f;

    void Start()
    {
        pathList = GameObject.Find("PathList").transform;
        sectionCount = platformIntervalTurn-1;
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
        }
        else
        {
            // Normal path piece
            prefabToSpawn = pathSections[Random.Range(0, pathSections.Length)];

            if (nextIsTurn)
            {
                // Platform immediately before a turn: force X = 0
                Vector3 euler = spawnRotation.eulerAngles;
                euler.x = 0f;
                spawnRotation = Quaternion.Euler(euler);

                lastTilt = 0f;
            }
            else
            {
                // Normal platform, not before a turn
                float tiltToApply = 0f;

                if (Random.value < 0.8f)
                {
                    float proposedTilt = Random.value < 0.5f ? -15f : 15f;

                    // Only force 0 if lastTilt is opposite of proposedTilt
                    if ((lastTilt == 15f && proposedTilt == -15f) || (lastTilt == -15f && proposedTilt == 15f))
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
