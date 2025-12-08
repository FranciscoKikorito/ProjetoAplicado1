using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject tube;
    public GameObject robot;

    [Header("Tube Spawn Points")]
    public List<Transform> tubePossibleSpawnsList;

    [Header("Robot Spawn Points")]
    public List<Transform> robotPossibleSpawnsList;

    private int lastTubeIndex = -1;
    private int lastRobotIndex = -1;

    void Start()
    {
        SpawnObstacle();
    }

    private void SpawnObstacle()
    {
        bool spawnTube = (Random.value < 0.5f);

        if (spawnTube)
        {
            SpawnTubeObstacle();
        }
        else
        {
            SpawnRobotObstacle();
        }
    }

    //  TUBE LOGIC  //
    private void SpawnTubeObstacle()
    {
        if (tube == null)
        {
            Debug.LogWarning("ObstacleSpawner: Tube prefab not assigned!");
            return;
        }

        if (tubePossibleSpawnsList.Count == 0)
        {
            Debug.LogWarning("ObstacleSpawner: No tube spawn points assigned!");
            return;
        }

        int randomIndex;

        // not spawn the same position twice
        do
        {
            randomIndex = Random.Range(0, tubePossibleSpawnsList.Count);
        } while (tubePossibleSpawnsList.Count > 1 && randomIndex == lastTubeIndex);

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

    //  ROBOT LOGIC //
    private void SpawnRobotObstacle()
    {
        if (robot == null)
        {
            Debug.LogWarning("ObstacleSpawner: Robot prefab not assigned!");
            return;
        }

        if (robotPossibleSpawnsList.Count == 0)
        {
            Debug.LogWarning("ObstacleSpawner: No robot spawn points assigned!");
            return;
        }

        int randomIndex;

        // not spawn the same position twice
        do
        {
            randomIndex = Random.Range(0, robotPossibleSpawnsList.Count);
        } while (robotPossibleSpawnsList.Count > 1 && randomIndex == lastRobotIndex);

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
}
