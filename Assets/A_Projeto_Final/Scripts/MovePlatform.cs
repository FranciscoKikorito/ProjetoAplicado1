using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    private Vector3 moveDirection = new Vector3(0, 0, -10);

    void Update()
    {
        transform.position += moveDirection * Time.deltaTime;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }
    /*private void OnTriggerEnter(Collider other)
      {
          if (other.CompareTag("PlayerFront"))
          {
              Debug.Log("RightTurnTrigger activated");
              moveDirection = new Vector3(-10, 0, 0);
          }
      }*/
}
