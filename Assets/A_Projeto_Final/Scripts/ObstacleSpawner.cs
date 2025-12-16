using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
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

    public bool canSpawnFan = true;
    public bool canSpawnRobot = true;

    public bool iteratedEnough = false;

    private MovePlatform pathList;


    void Start()
    {

        Transform platform = transform;
        //Debug.Log(platform);

        int myIndex = platform.GetSiblingIndex();
        pathList = GameObject.Find("PathList").GetComponent<MovePlatform>();

        if (myIndex > 0)
        {
            Transform prevPlatform = pathList.transform.GetChild(myIndex - 1);
            ObstacleSpawner prevSpawner = prevPlatform.GetComponent<ObstacleSpawner>();

            //Debug.Log("prevPlatform: " + prevPlatform);
            //Debug.Log("prevSpwaner: " + prevSpawner);
            if (prevSpawner != null)
            {
                canSpawnFan = prevSpawner.checkIfPreviousWasFan();
                //Debug.Log("CAN SPAWN FAN?: " + canSpawnFan);
            }

        }

        if (Mathf.Abs(platform.rotation.eulerAngles.x) == 15 || Mathf.Abs(platform.rotation.eulerAngles.x) == 345) {
            //Debug.Log(platform.rotation.eulerAngles.x);
            canSpawnFan = false;
        }    

        pathList = GameObject.Find("PathList").GetComponent<MovePlatform>();
        PathDestroy checks = pathList.GetComponentInChildren<PathDestroy>();

        iteratedEnough = checks.checkFans();
        canSpawnRobot = checks.checkRobots();
        //Debug.Log("Spawn fan? : " + canSpawnFan);
        //Debug.Log("Iterated enough? : " + iteratedEnough);

        SpawnObstacle();
    }

    private void SpawnObstacle()
    {
        float rand = Random.value;

        if (rand < 0.50f)
        {
            if (canSpawnFan)
                SpawnTubeObstacle();
            else
                return;
        }
        else if (rand < 0.70f)
        {
            if (canSpawnRobot)
                SpawnRobotObstacle();
            else
                SpawnTubeObstacle();
        }
        else
        {
            if (canSpawnFan && iteratedEnough)
                SpawnFanObstacle();
            else
                SpawnTubeObstacle();
        }
    }



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
        canSpawnFan = false;
        Instantiate(fan, spawnPoint.position, spawnPoint.rotation, spawnPoint);
    }

    public bool checkIfPreviousWasFan()
    {
        return canSpawnFan;
    }
}

