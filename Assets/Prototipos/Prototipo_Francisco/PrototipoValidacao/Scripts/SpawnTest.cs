using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    public GameObject[] pathSections;
    [SerializeField] private GameObject startSpawnPlatform;
    //private Vector3 spawnPosition = GameObject.Find("PathLineEmpty").transform.position;

    void Start()
    {

    }

    private void OnTriggerExit(Collider other)
    {
        print("SpawnedNextPlatform");
        if (other.gameObject.CompareTag("Trigger"))
        {
            int randomIndex = Random.Range(0, pathSections.Length);
            Vector3 spawnPosition = GameObject.Find("PathLineEmpty").transform.position;
            Instantiate(pathSections[randomIndex], spawnPosition, Quaternion.identity);
        }
    }
}
