using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionTriggerProcedural : MonoBehaviour
{
    [SerializeField] private GameObject[] pathSections;

    private Vector3 spawnPosition = new Vector3(-1.163f, 0, 58.50f);

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            int randomIndex = Random.Range(0, pathSections.Length);
            Instantiate(pathSections[randomIndex], spawnPosition, Quaternion.identity);
        }
    }
}
