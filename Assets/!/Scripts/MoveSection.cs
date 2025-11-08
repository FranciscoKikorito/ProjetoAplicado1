using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Move : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

        transform.position += new Vector3(20, 0, 0) * Time.deltaTime;

    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }
}