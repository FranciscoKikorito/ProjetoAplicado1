using System.Collections.Generic;
using UnityEngine;

public class BackgroundHouseSpawner : MonoBehaviour
{
    public GameObject backgroundHouses;
    public List<Transform> spawnPointsList;

    void Start()
    {
        foreach (Transform p in spawnPointsList)
            Instantiate(backgroundHouses, p.position, p.rotation, p);
    }

}
