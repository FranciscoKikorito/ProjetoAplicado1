using UnityEngine;

public class PathDestroy : MonoBehaviour
{
    [SerializeField] private int maxPlatforms;
    private int counter = 0;
    public bool startRobots = false;
    public bool startFans = false;

    void Update()
    {
        if (transform.childCount > maxPlatforms)
        {
            //GetChild(1) because directional light is a child of the list so it follows the path
            Transform firstChild = transform.GetChild(1);
            Destroy(firstChild.gameObject);
            counter++;
        }

        if (counter > 0)
        {
            startRobots = true;
        }
        if (counter > 4)
        {
            startFans = true;
        }
    }


    //this is called by obstacle spawner, with this we can define sections without certain obstacles by counting how many have been destroyed
    public bool checkRobots()
    {
        return startRobots;
    }

    public bool checkFans()
    {
        return startFans;
    }

}
