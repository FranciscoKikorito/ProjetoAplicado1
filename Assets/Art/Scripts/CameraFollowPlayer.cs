using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;

    void Update()
    {
        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            target.position.z - distance
        );
    }
}
