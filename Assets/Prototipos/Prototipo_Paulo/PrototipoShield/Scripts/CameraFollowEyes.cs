using UnityEngine;

public class CameraFollowEyes : MonoBehaviour
{
    public Transform eye;      
    public float rotationSpeed = 10f; 

    void LateUpdate()
    {
        if (eye == null) return;
        
        Vector3 lookDirection = eye.forward;
        
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}