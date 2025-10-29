using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public GameObject white;
    public GameObject black;

    public float speed = 4f;
    private bool isWhiteActive = true;

    private Renderer whiteRenderer;
    private Renderer blackRenderer;

    void Start()
    {
        whiteRenderer = white.GetComponent<Renderer>();
        blackRenderer = black.GetComponent<Renderer>();

        ApplyState();
    }

    void Update()
    {
        // Movimento do grupotodo
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0f, v);

        if (dir.sqrMagnitude > 0.001f)
        {
            transform.position += dir.normalized * speed * Time.deltaTime;

            // Faz o "inactivo" ficar sempre um pouco atrás do activo
            UpdateFollowerPosition(dir);
        }

        // Troca de personagem activo
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isWhiteActive = !isWhiteActive;
            ApplyState();
        }
    }

    void ApplyState()
    {
        if (isWhiteActive)
        {
            // White activo
            white.transform.localScale = Vector3.one * 1.2f;
            whiteRenderer.material.color = new Color(1f, 1f, 1f, 1f);

            // Black inactivo (menor + transparente)
            black.transform.localScale = Vector3.one * 0.8f;
            blackRenderer.material.color = new Color(0f, 0f, 0f, 0.4f);
        }
        else
        {
            // White inactivo
            white.transform.localScale = Vector3.one * 0.8f;
            whiteRenderer.material.color = new Color(1f, 1f, 1f, 0.4f);

            // Black activo
            black.transform.localScale = Vector3.one * 1.2f;
            blackRenderer.material.color = new Color(0f, 0f, 0f, 1f);
        }
    }

    void UpdateFollowerPosition(Vector3 dir)
    {
        GameObject active = isWhiteActive ? white : black;
        GameObject inactive = isWhiteActive ? black : white;

        // Mantém o inactivo atrás do activo, um pouco afastado
        Vector3 offset = -dir.normalized * 1.2f; // distância atrás
        inactive.transform.position = Vector3.Lerp(
            inactive.transform.position,
            active.transform.position + offset,
            Time.deltaTime * 5f // suaviza movimento
        );
    }
}