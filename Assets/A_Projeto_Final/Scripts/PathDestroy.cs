using UnityEngine;

public class PathDestroy : MonoBehaviour
{
    [SerializeField] private int maxPlatforms;

    void Update()
    {
        if (transform.childCount > maxPlatforms)
        {
            Transform firstChild = transform.GetChild(0);
            Destroy(firstChild.gameObject);
        }
    }
}
