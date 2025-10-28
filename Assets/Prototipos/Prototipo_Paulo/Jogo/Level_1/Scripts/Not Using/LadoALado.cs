using UnityEngine;

public class LadoALado : MonoBehaviour
{
    public GameObject white;
    public GameObject black;

    public float speed = 5f;              // movimento automático para a frente
    public float swapSmooth = 5f;         // suavidade da transição visual
    public float distanceBetween = 1.2f;  // distância entre os dois jogadores

    private bool isWhiteActive = true;

    private Renderer whiteRenderer;
    private Renderer blackRenderer;

    private bool stopped = false;

    void Start()
    {
        whiteRenderer = white.GetComponent<Renderer>();
        blackRenderer = black.GetComponent<Renderer>();

        // Define posições fixas (lado a lado)
        white.transform.localPosition = Vector3.left * (distanceBetween / 2f);
        black.transform.localPosition = Vector3.right * (distanceBetween / 2f);

        ApplyVisualState();
    }

    void Update()
    {
        if (stopped) return;

        // Movimento automático
        transform.position += Vector3.forward * speed * Time.deltaTime;

        // Alternar personagem ativa (espaço ou clique)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            isWhiteActive = !isWhiteActive;
            ApplyVisualState();
        }

        // Transição suave visual (escala e transparência)
        UpdateVisualTransition();
    }
    public bool IsWhiteActive()
    {
        return isWhiteActive;
    }
    void ApplyVisualState()
    {
        // Apenas troca o estado alvo (não posição)
        if (isWhiteActive)
        {
            targetWhiteScale = Vector3.one * 1.2f;
            targetBlackScale = Vector3.one * 0.8f;

            targetWhiteColor = new Color(1f, 1f, 1f, 1f);
            targetBlackColor = new Color(0f, 0f, 0f, 0.5f);
        }
        else
        {
            targetWhiteScale = Vector3.one * 0.8f;
            targetBlackScale = Vector3.one * 1.2f;

            targetWhiteColor = new Color(1f, 1f, 1f, 0.4f);
            targetBlackColor = new Color(0f, 0f, 0f, 1f);
        }
    }

    private Vector3 targetWhiteScale;
    private Vector3 targetBlackScale;
    private Color targetWhiteColor;
    private Color targetBlackColor;

    void UpdateVisualTransition()
    {
        // Escala suave
        white.transform.localScale = Vector3.Lerp(
            white.transform.localScale,
            targetWhiteScale,
            Time.deltaTime * swapSmooth
        );

        black.transform.localScale = Vector3.Lerp(
            black.transform.localScale,
            targetBlackScale,
            Time.deltaTime * swapSmooth
        );

        // Cor suave
        whiteRenderer.material.color = Color.Lerp(
            whiteRenderer.material.color,
            targetWhiteColor,
            Time.deltaTime * swapSmooth
        );

        blackRenderer.material.color = Color.Lerp(
            blackRenderer.material.color,
            targetBlackColor,
            Time.deltaTime * swapSmooth
        );
    }

    public void StopMovement()
    {
        stopped = true;
    }
}
