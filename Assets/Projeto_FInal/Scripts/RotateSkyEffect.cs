using UnityEngine;

public class RotateSkyEffect : MonoBehaviour
{
    public float rotationSpeed;

    void Update()
    {
        transform.Rotate(0f, -rotationSpeed * Time.deltaTime, 0f);
    }
}
