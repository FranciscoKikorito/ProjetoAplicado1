using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject tube;
    public GameObject robot;
    public GameObject fan;

    public List<Transform> tubePossibleSpawnsList;
    public List<Transform> robotPossibleSpawnsList;
    public List<Transform> fanPossibleSpawnsList;

    private int lastTubeIndex = -1;
    private int lastRobotIndex = -1;
    private int lastFanIndex = -1;

    void Start()
    {
        SpawnObstacle();
    }
    
    private void SpawnObstacle()
    {
        float rand = Random.value;

        if (rand < 0.60f)
        {
            SpawnTubeObstacle();
        }
        else if (rand < 0.75f)
        {
            SpawnRobotObstacle();
        }
        else
        {
            SpawnFanObstacle();
        }
    }

    // LOGICA TUBOS //
    private void SpawnTubeObstacle()
    {
        if (tube == null || tubePossibleSpawnsList.Count == 0) return;

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, tubePossibleSpawnsList.Count);
        } 
        while (tubePossibleSpawnsList.Count > 1 && randomIndex == lastTubeIndex);

        lastTubeIndex = randomIndex;
        Transform chosenPoint = tubePossibleSpawnsList[randomIndex];

        if (chosenPoint.childCount > 0)
        {
            for (int i = 0; i < chosenPoint.childCount; i++)
                SpawnTube(chosenPoint.GetChild(i));
        }
        else
        {
            SpawnTube(chosenPoint);
        }
    }

    private void SpawnTube(Transform spawnPoint)
    {
        Instantiate(tube, spawnPoint.position, spawnPoint.rotation, spawnPoint);
    }


    // LOGICA ROBOS //
    private void SpawnRobotObstacle()
    {
        if (robot == null || robotPossibleSpawnsList.Count == 0) return;

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, robotPossibleSpawnsList.Count);
        } 
        while (robotPossibleSpawnsList.Count > 1 && randomIndex == lastRobotIndex);

        lastRobotIndex = randomIndex;
        Transform chosenPoint = robotPossibleSpawnsList[randomIndex];

        if (chosenPoint.childCount > 0)
        {
            for (int i = 0; i < chosenPoint.childCount; i++)
                SpawnRobot(chosenPoint.GetChild(i));
        }
        else
        {
            SpawnRobot(chosenPoint);
        }
    }

    private void SpawnRobot(Transform spawnPoint)
    {
        Instantiate(robot, spawnPoint.position, spawnPoint.rotation, spawnPoint);
    }



    // LOGICA FANS //
    private void SpawnFanObstacle()
    {
        if (fan == null || fanPossibleSpawnsList.Count == 0) return;

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, fanPossibleSpawnsList.Count);
        } 
        while (fanPossibleSpawnsList.Count > 1 && randomIndex == lastFanIndex);

        lastFanIndex = randomIndex;
        Transform chosenPoint = fanPossibleSpawnsList[randomIndex];

        if (chosenPoint.childCount > 0)
        {
            for (int i = 0; i < chosenPoint.childCount; i++)
                SpawnFan(chosenPoint.GetChild(i));
        }
        else
        {
            SpawnFan(chosenPoint);
        }
    }

    private void SpawnFan(Transform spawnPoint)
    {
        Instantiate(fan, spawnPoint.position, spawnPoint.rotation, spawnPoint);
    }
}
