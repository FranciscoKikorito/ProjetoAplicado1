using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.UI;
using UnityEngine.Rendering;
public class GameStartController : MonoBehaviour
{
    [Header("Player")] 
    public Animator playerAnimator;
    public static bool canJump = false;
    public static bool canShield = false;
        
    [Header("Gameplay")] 
    public MovePlatform[] allPlatforms;
    public float platformStartSpeed = -10f;
    
    [Header("Cinemachine")]
    public CinemachineCamera introCam;
    public CinemachineCamera gameplayCam;
    
    [Header("UI")] 
    public GameObject pressStartUI;
    
    [Header("Game Over / Fade")] 
    public RectTransform gameOverUI;
    public float gameOverDisplayTime = 2.0f;
    public CanvasGroup fadeOverlay;
    public float fadeDuration = 3f;
    public bool isGameOver = false; // Impede inputs durante o fade
    private Vector2 originalGameOverPos;
    public static bool inputLocked = false;
    
    [Header("WIN")]
    public GameObject vfxPrefab;
    public GameObject player;
    public SlopesAndJumping sap;
    public MovePlatform mp;
    public Camera winCamera;
    public UnityEngine.Playables.PlayableDirector winDirector;

    [Header("Game Over - Post Processing")]
    public Volume glitchPostProcessVolume; 
    public float glitchRampUpTime = 0.5f; // Tempo para ativar os efeitos visuais

    [Header("Audio")] 
    public MusicManager musicManager;
    public AudioSource sfxSource; 
    //public AudioClip gameOverSFX;
    public AudioClip finalOuchPlayerSFX; 
    
    private bool gameStarted = false;
    private bool animationPlayed = false;
    
    void Start()
    {
        if (gameOverUI != null) 
        {
            gameOverUI.gameObject.SetActive(false);
            originalGameOverPos = gameOverUI.anchoredPosition;
        }

        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            fadeOverlay.blocksRaycasts = false;
        }
        if (glitchPostProcessVolume != null)
        {
            glitchPostProcessVolume.weight = 0f;
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
        introCam.Priority = 20;
        gameplayCam.Priority = 10;
        pressStartUI.SetActive(true);

        isGameOver = false;
        inputLocked = false;

        if (musicManager != null)
            musicManager.PlayIntroMusic();
    }

    void Update()
    {
        if (isGameOver || inputLocked) return;

        if (!gameStarted)
        {

            if (Input.GetMouseButtonDown(0))
            {
                gameStarted = true;

                // troca de câmeras →
                introCam.Priority = 10;
                gameplayCam.Priority = 20;
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
                if (GameTimerManager.Instance != null)
                {
                    GameTimerManager.Instance.StartTimer();
                }
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
        
        if (musicManager != null)
        {
            musicManager.StopMusic(); 
        }
        if (sfxSource != null && finalOuchPlayerSFX != null)
        {
            sfxSource.PlayOneShot(finalOuchPlayerSFX);
        }
        
        foreach (var p in allPlatforms)
        {
            p.SetMoveDirection(Vector3.zero);
        }

        StartCoroutine(GameOverSequence());
    }
    
    public void TriggerWin()
    {
        if (isGameOver) return;

        isGameOver = true;
        inputLocked = true;

        canJump = false;
        canShield = false;

        if (musicManager != null)
            musicManager.StopMusic();

        foreach (var p in allPlatforms)
            p.SetMoveDirection(Vector3.zero);
            mp.enabled = false;
            sap.enabled = false;

        StartCoroutine(WinSequence());
    }
    IEnumerator WinSequence()
    {
        if (player != null && vfxPrefab != null)
        {
            GameObject vfxInstance = Instantiate(vfxPrefab, player.transform.position, player.transform.rotation);
            Destroy(vfxInstance, 3f);
        }

        if (player != null)
            player.gameObject.SetActive(false);
        

        float timer = 0f;

        if (fadeOverlay != null)
            fadeOverlay.blocksRaycasts = true;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            if (fadeOverlay != null)
                fadeOverlay.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        if (fadeOverlay != null)
            fadeOverlay.alpha = 1f;

        if (introCam) introCam.gameObject.SetActive(false);
        if (gameplayCam) gameplayCam.gameObject.SetActive(false);
        if (winCamera) winCamera.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        timer = 0f;
        if (winDirector)
            winDirector.Play();

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            if (fadeOverlay != null)
                fadeOverlay.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            fadeOverlay.blocksRaycasts = false;
        }

    }


    IEnumerator GameOverSequence()
    {
        
        
        float timer = 0f;
        if (fadeOverlay != null) fadeOverlay.blocksRaycasts = true;

        // Fazemos o fade do ecrã preto E o fade dos efeitos de glitch ao mesmo tempo
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            if (fadeOverlay != null) 
                fadeOverlay.alpha = Mathf.Lerp(0f, 1f, progress);

            if (glitchPostProcessVolume != null)
                glitchPostProcessVolume.weight = Mathf.Lerp(0f, 1f, progress / glitchRampUpTime);

            yield return null;
        }
        if (fadeOverlay != null) fadeOverlay.alpha = 1f;
        if (glitchPostProcessVolume != null) glitchPostProcessVolume.weight = 1f;
        
        yield return new WaitForSeconds(0.3f);
        
        if (gameOverUI != null)
        {
            gameOverUI.gameObject.SetActive(true);
            //if (sfxSource != null && gameOverSFX != null)
            //{
            //    sfxSource.PlayOneShot(gameOverSFX);
            //}
            Vector3 startScale = new Vector3(4f, 4f, 4f); 
            Vector3 endScale = Vector3.one; // Escala normal (1x)

            float slamDuration = 0.15f; 
            float slamTimer = 0f;
            
            while (slamTimer < slamDuration)
            {
                slamTimer += Time.deltaTime;
                // Lerp simples na escala para bater forte
                gameOverUI.localScale = Vector3.Lerp(startScale, endScale, slamTimer / slamDuration);
                yield return null;
            }
            gameOverUI.localScale = endScale;
        }
        
        float displayTimer = 0f;
        float shakeIntensity = 0.3f; // Quão forte ele treme (em pixeis)

        while (displayTimer < gameOverDisplayTime)
        {
            displayTimer += Time.deltaTime;

            if (gameOverUI != null)
            {
                // Gera uma posição aleatória perto do centro em cada frame
                float randomX = Random.Range(-shakeIntensity, shakeIntensity);
                float randomY = Random.Range(-shakeIntensity, shakeIntensity);
                // Aplica o tremor
                gameOverUI.anchoredPosition = originalGameOverPos + new Vector2(randomX, randomY);
                
                // Opcional: Fazer a escala tremer ligeiramente também para parecer instável
                float randomScale = Random.Range(0.98f, 1f);
                gameOverUI.localScale = new Vector3(randomScale, randomScale, 1f);
            }

            yield return null; 
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}