using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SectionTriggerMultiple : MonoBehaviour
{
    public GameObject RS_Basic;
    public GameObject RS_Enemy3;
    public GameObject RS_Bridge;

    private int spawnIndex = 0;
    private bool hasSpawned = false;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Trigger")) return;

        if (!hasSpawned)
        {
            GameObject prefabToSpawn = null;
            switch (spawnIndex)
            {
                case 0: prefabToSpawn = RS_Basic; break;
                case 1: prefabToSpawn = RS_Enemy3; break;
                case 2: prefabToSpawn = RS_Bridge; break;
            }

            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, new Vector3(-36, 0, 0), Quaternion.identity);
            }

            spawnIndex = (spawnIndex + 1) % 3;
            hasSpawned = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Trigger"))
            hasSpawned = false; // reset when the object leaves trigger
    }
}
