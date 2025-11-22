using UnityEngine;

public class Camara : MonoBehaviour
{
    public Transform player;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 2f, -5f);
    public float smoothSpeed = 5f;

    [Header("Slope Settings")]
    public float maxTilt = 25f;      // quanto a camara pode inclinar no máximo
    public float maxSlopeAngle = 45f; // inclinação máxima que consideras no jogo
    public LayerMask groundLayer;

    private void LateUpdate()
    {
        float tilt = 0f;

        // Faz raycast para baixo e obtém a normal da superfície
        if (Physics.Raycast(player.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 2f, groundLayer))
        {
            // Calcula o ângulo real do chão
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            // Converte a inclinação do chão na inclinação da câmera
            tilt = Mathf.Lerp(0f, maxTilt, slopeAngle / maxSlopeAngle);
        }

        // MOVIMENTO
        Vector3 desiredPos = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);

        // ROTAÇÃO (inclinação automática)
        Vector3 angles = transform.eulerAngles;
        angles.x = Mathf.LerpAngle(angles.x, tilt, Time.deltaTime * smoothSpeed);
        transform.eulerAngles = angles;
    }
}