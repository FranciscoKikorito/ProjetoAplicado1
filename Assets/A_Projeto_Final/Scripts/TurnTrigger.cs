using UnityEngine;

public class TurnTrigger : MonoBehaviour
{
    private MovePlatform parentMover;

    void Start()
    {
        parentMover = GameObject.Find("PathList").GetComponent<MovePlatform>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TurnTrigger"))
        {
            Vector3 newDirection = other.transform.forward * 10f;
            parentMover.SetMoveDirection(-newDirection);

            Quaternion newRotation = Quaternion.LookRotation(newDirection, Vector3.up);
            transform.rotation = newRotation;
        }
    }

}
