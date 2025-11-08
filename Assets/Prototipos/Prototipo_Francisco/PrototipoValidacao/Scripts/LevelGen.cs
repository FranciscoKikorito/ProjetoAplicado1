using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour
{
    [SerializeField] public GameObject[] pathSections;
    [SerializeField] private GameObject furthestPlatform;

    private Transform pathList;
    void Start()
    {
        pathList = GameObject.Find("PathList").transform;
    }

    private void OnTriggerExit(Collider other)
    {
        print("SpawnedNextPlatform");
        if (other.gameObject.CompareTag("Trigger"))
        {
            int randomIndex = Random.Range(0, pathSections.Length);
            Vector3 spawnPosition = furthestPlatform.transform.Find("NextSectionPoint").position;
            GameObject newPlatform = Instantiate(pathSections[randomIndex], spawnPosition, Quaternion.identity, pathList);
            furthestPlatform = newPlatform;
        }
    }
}
