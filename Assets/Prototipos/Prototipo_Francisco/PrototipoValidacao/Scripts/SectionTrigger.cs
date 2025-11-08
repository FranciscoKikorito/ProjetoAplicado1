using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] pathSections;

    [SerializeField] private Vector3 spawnPosition = new Vector3(-1.163f, 0, 38);

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            int randomIndex = Random.Range(0, pathSections.Length);
            Instantiate(pathSections[randomIndex], spawnPosition, Quaternion.identity);
        }
    }
}
