using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 4f;
    [HideInInspector] public bool active = false;

    void Update()
    {
        if (!active) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0f, v);
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.position += dir.normalized * speed * Time.deltaTime;
        }
    }
}