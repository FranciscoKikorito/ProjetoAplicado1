using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;
using UnityEngine.UI;

public class GameStartController : MonoBehaviour
{
    [Header("Player")] public Animator playerAnimator;
    public static bool canJump = false;

    [Header("Gameplay")] public MovePlatform[] allPlatforms;
    public float platformStartSpeed = -10f;

    [Header("Cameras")] public GameObject introCAM; // câmera da intro (ativa no início)
    public GameObject gameplayCAM; // câmera da gameplay (ativa depois do clique)

    [Header("UI")] public GameObject pressStartUI;

    [Header("Game Over / Fade")] public RectTransform gameOverUI;
    public float gameOverDisplayTime = 2.0f;
    public CanvasGroup fadeOverlay;
    public float fadeDuration = 1.5f;
    public bool isGameOver = false; // Impede inputs durante o fade

    [Header("Audio")] public MusicManager musicManager;

    private bool gameStarted = false;
    private bool animationPlayed = false;

    void Start()
    {
        if (gameOverUI != null) gameOverUI.gameObject.SetActive(false);

        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            fadeOverlay.blocksRaycasts = false;
        }

        // parar plataformas
        foreach (var p in allPlatforms)
        {
            //Debug.Log(p);
            p.SetMoveDirection(Vector3.zero);
        }

        // garantir idle inicial
        playerAnimator.Play("Idle_Start");

        // Ativa introCAM + UI
        introCAM.SetActive(true);
        gameplayCAM.SetActive(false);
        pressStartUI.SetActive(true);

        isGameOver = false;

        if (musicManager != null)
            musicManager.PlayIntroMusic();
    }

    void Update()
    {
        if (isGameOver) return;

        if (!gameStarted)
        {

            if (Input.GetMouseButtonDown(0))
            {
                gameStarted = true;

                // troca de câmeras →
                introCAM.SetActive(false);
                gameplayCAM.SetActive(true);
                // esconde a UI
                pressStartUI.SetActive(false);
                // start do player
                playerAnimator.SetTrigger("StartGame");
            }

            return;
        }

        // quando a animação StandUp acabar , começa o jogo
        AnimatorStateInfo state = playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (!animationPlayed && state.IsName("Rig|Run"))
        {
            animationPlayed = true;
            canJump = true;
            foreach (var p in allPlatforms)
            {
                p.SetMoveDirection(Vector3.forward * platformStartSpeed);
            }

            if (musicManager != null)
                musicManager.PlayGameplayMusic();
        }
    }

    // --- GAME OVER ---
    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        canJump = false;

        foreach (var p in allPlatforms)
        {
            p.SetMoveDirection(Vector3.zero);
        }

        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        float fadeTimer = 0f;
        if (fadeOverlay != null)
        {
            fadeOverlay.blocksRaycasts = true;
            while (fadeTimer < fadeDuration)
            {
                fadeTimer += Time.deltaTime;
                fadeOverlay.alpha = Mathf.Lerp(0f, 1f, fadeTimer / fadeDuration);
                yield return null;
            }

            fadeOverlay.alpha = 1f;
            yield return new WaitForSeconds(0.2f);

            if (gameOverUI != null)
            {
                gameOverUI.gameObject.SetActive(true);

                Vector2 startPos = new Vector2(1500f, 0f);
                Vector2 endPos = Vector2.zero;

                float slideDuration = 0.8f;
                float timer = 0f;

                while (timer < slideDuration)
                {
                    timer += Time.deltaTime;
                    float percentage = timer / slideDuration;

                    float smooth = Mathf.SmoothStep(0f, 1f, percentage);
                    gameOverUI.anchoredPosition = Vector2.Lerp(startPos, endPos, smooth);
                    yield return null;
                }

                gameOverUI.anchoredPosition = endPos;
            }

            yield return new WaitForSeconds(gameOverDisplayTime);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}