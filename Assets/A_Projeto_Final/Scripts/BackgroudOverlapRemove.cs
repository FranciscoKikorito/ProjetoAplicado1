using UnityEngine;

public class BackgroundOverlapRemove : MonoBehaviour
{
    private MovePlatform pathList;

    void Start()
    {
        Transform platform = transform;
        int myIndex = platform.GetSiblingIndex();
        //Debug.Log("myIndex: " + myIndex);

        pathList = GameObject.Find("PathList").GetComponent<MovePlatform>();
        Transform prevPlatform = pathList.transform.GetChild(myIndex - 1);

        Transform bgPoints = prevPlatform.Find("ProceduralBuildingsPointList/BackgroundPointsList");
        Destroy(bgPoints.gameObject);
    }
}
