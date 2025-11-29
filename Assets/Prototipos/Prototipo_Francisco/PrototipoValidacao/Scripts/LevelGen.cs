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
    private int sectionCount = 0;

    void Start()
    {
        pathList = GameObject.Find("PathList").transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            sectionCount++;

            GameObject prefabToSpawn;

            bool isTurn = sectionCount % platformIntervalTurn == 0;

            if (isTurn)
                prefabToSpawn = Random.value < 0.5f ? turnLeftSection : turnRightSection;
            else
                prefabToSpawn = pathSections[Random.Range(0, pathSections.Length)];

            Vector3 spawnPosition = furthestPlatform.transform.Find("NextSectionPoint").position;

            Quaternion rot = furthestPlatform.transform.Find("NextSectionPoint").rotation;
            GameObject newPlatform = Instantiate(prefabToSpawn, spawnPosition, rot, pathList);
            furthestPlatform = newPlatform;
        }
    }
}
