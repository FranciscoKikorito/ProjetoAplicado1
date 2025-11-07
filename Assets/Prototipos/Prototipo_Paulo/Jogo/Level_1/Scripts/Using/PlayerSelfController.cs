using UnityEngine;

public class PlayerSelfController1 : MonoBehaviour
{
    public GameObject white;
    public GameObject black;

    public float speed = 5f;
    public float swapSmooth = 5f;
    public float distanceBetween = 1.2f;

    private bool isWhiteActive = true;
    private Renderer whiteRenderer;
    private Renderer blackRenderer;

    private bool stopped = false;
    private TimingPoint currentTimingPoint;

    private Vector3 targetWhiteScale;
    private Vector3 targetBlackScale;
    private Color targetWhiteColor;
    private Color targetBlackColor;
    private Vector3 targetWhitePosition;
    private Vector3 targetBlackPosition;

    void Start()
    {
        whiteRenderer = white.GetComponent<Renderer>();
        blackRenderer = black.GetComponent<Renderer>();

        ApplyVisualState();
        ApplyPositionState();
    }

    void Update()
    {
        if (stopped) return;

        // Movimento automático
        transform.position += Vector3.forward * speed * Time.deltaTime;

        HandleInput();

        // Atualiza visual suavemente
        UpdateVisualTransition();
    }

    void HandleInput()
    {
        // 🖱️ Clique direito → trocar personagem
        if (Input.GetMouseButtonDown(1))
        {
            isWhiteActive = !isWhiteActive;
            ApplyVisualState();
            ApplyPositionState();
        }

        // 🖱️ Clique esquerdo → tentar ação (construir ou destruir)
        if (Input.GetMouseButtonDown(0))
        {
            TryPerformAction();
        }
    }

    // 🔹 Alternar visuais
    void ApplyVisualState()
    {
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

    // 🔹 Alternar posição (frente e trás)
    void ApplyPositionState()
    {
        if (isWhiteActive)
        {
            targetWhitePosition = Vector3.forward * (distanceBetween / 2f);
            targetBlackPosition = Vector3.back * (distanceBetween / 2f);
        }
        else
        {
            targetWhitePosition = Vector3.back * (distanceBetween / 2f);
            targetBlackPosition = Vector3.forward * (distanceBetween / 2f);
        }
    }

    // 🔹 Atualização visual suave
    void UpdateVisualTransition()
    {
        white.transform.localScale = Vector3.Lerp(white.transform.localScale, targetWhiteScale, Time.deltaTime * swapSmooth);
        black.transform.localScale = Vector3.Lerp(black.transform.localScale, targetBlackScale, Time.deltaTime * swapSmooth);

        whiteRenderer.material.color = Color.Lerp(whiteRenderer.material.color, targetWhiteColor, Time.deltaTime * swapSmooth);
        blackRenderer.material.color = Color.Lerp(blackRenderer.material.color, targetBlackColor, Time.deltaTime * swapSmooth);

        white.transform.localPosition = Vector3.Lerp(white.transform.localPosition, targetWhitePosition, Time.deltaTime * swapSmooth);
        black.transform.localPosition = Vector3.Lerp(black.transform.localPosition, targetBlackPosition, Time.deltaTime * swapSmooth);
    }

    // 🔹 Ação (construir / destruir)
    private void TryPerformAction()
    {
        if (currentTimingPoint == null)
        {
            return;
        }

        if (isWhiteActive && currentTimingPoint.actionType == TimingPoint.ActionType.Build)
        {
            
            BuildBridge(currentTimingPoint);
        }
        else if (!isWhiteActive && currentTimingPoint.actionType == TimingPoint.ActionType.Destroy)
        {
            DestroyWall(currentTimingPoint);
        }
        else
        {
            Debug.Log("⚠️ Personagem errado para esta ação!");
        }

        currentTimingPoint = null;
    }

    // ✅ Ativa a ponte pré-colocada na cena
    private void BuildBridge(TimingPoint tp)
    {
        if (tp.associatedObject != null)
        {
            tp.associatedObject.SetActive(true);
            StartCoroutine(tp.AnimateBridgeRise(tp.associatedObject));
        }
    }

    private void DestroyWall(TimingPoint tp)
    {
        if (tp.associatedObject == null) return;

        StartCoroutine(tp.AnimateWallFall(tp.associatedObject, () =>
        {
            tp.SpawnDestroyParticles(tp.associatedObject.transform.position);
            Destroy(tp.associatedObject);
        }));
    }

    public void StopMovement()
    {
        stopped = true;
    }

    // 🔹 Detectar entrada e saída dos pontos
    private void OnTriggerEnter(Collider other)
    {
        TimingPoint tp = other.GetComponent<TimingPoint>();
        if (tp != null)
        {
            currentTimingPoint = tp;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<TimingPoint>() == currentTimingPoint)
        {
            currentTimingPoint = null;
        }
    }
}
