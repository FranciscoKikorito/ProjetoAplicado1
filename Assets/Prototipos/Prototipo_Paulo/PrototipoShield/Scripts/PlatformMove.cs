using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    public float baseSpeed = 10f;       // velocidade normal
    public float dashSpeed = 30f;       // velocidade durante dash
    private float currentSpeed;

    private Vector3 direction = Vector3.back;

    void Start()
    {
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        transform.position += direction * currentSpeed * Time.deltaTime;
    }

    public void SetDash(bool active)
    {
        currentSpeed = active ? dashSpeed : baseSpeed;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }
}