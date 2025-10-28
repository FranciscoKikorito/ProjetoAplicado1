using UnityEngine;

// script simples para a camera seguir o jogador activo.
// podes ligar a camera ao transform do White ou Black manualmente, ou controlar via PlayerSwitcher.
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 6f, -8f);
    public float smooth = 6f;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smooth);
        transform.LookAt(target.position + Vector3.up * 1.2f);
    }
}

