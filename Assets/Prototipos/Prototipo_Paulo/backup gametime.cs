using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class GameStartControllerBackup : MonoBehaviour
{
    [Header("Player")] 
    public Animator playerAnimator;
    public static bool canJump = false;
    public static bool canShield = false;

    [Header("Gameplay")] 
    public MovePlatform[] allPlatforms;
    public float platformStartSpeed = -10f;

    [Header("Cinemachine")] 
    public CinemachineCamera skyCam;
    public CinemachineCamera introCam;
    public CinemachineCamera gameplayCam;

    [Header("UI")] 
    public GameObject pressStartUI;
    public GameObject skyIntroUI;
    
    
    [Header("Game Over / Fade")] 
    public RectTransform gameOverUI;
    public float gameOverDisplayTime = 2.0f;
    public CanvasGroup fadeOverlay;
    public float fadeDuration = 1.5f;
    public bool isGameOver = false;
    private Vector2 originalGameOverPos;
    public static bool inputLocked = false;

    [Header("WIN")] 
    public Camera winCamera;
    public UnityEngine.Playables.PlayableDirector winDirector;

    [Header("Audio")] 
    public MusicManager musicManager;
    public AudioSource sfxSource;
    public AudioClip finalOuchPlayerSFX;

    [Header("Game Over - Post Processing")]
    public Volume glitchPostProcessVolume;

    public float glitchRampUpTime = 0.5f;

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
        
        if (glitchPostProcessVolume != null) glitchPostProcessVolume.weight = 0f;

        foreach (var p in allPlatforms) p.SetMoveDirection(Vector3.zero);
        if (playerAnimator != null) playerAnimator.Play("Idle_Start");
        
        if (skyIntroUI != null) skyIntroUI.SetActive(true);

        if (skyCam != null)
        {
            skyCam.gameObject.SetActive(true);
            skyCam.Priority = 100; 
        }
        
        if (introCam != null)
        {
            introCam.gameObject.SetActive(true);
            introCam.Priority = 50; 
        }

        if (gameplayCam != null)
        {
            gameplayCam.gameObject.SetActive(true);
            gameplayCam.Priority = 10;
        }
        
        if (pressStartUI != null) pressStartUI.SetActive(false);
        isGameOver = false;
        inputLocked = true;

        if (musicManager != null) musicManager.PlayIntroMusic();
        
        StartCoroutine(PlaySkyIntroSequence());
    }

    IEnumerator PlaySkyIntroSequence()
    {
        yield return new WaitForSeconds(3.0f);

        if (skyIntroUI != null) skyIntroUI.SetActive(false);

        yield return new WaitForSeconds(1.0f);
        
        if (skyCam != null)
        {
            skyCam.Priority = 0;
            skyCam.gameObject.SetActive(false);
        }
        
        if (pressStartUI != null) pressStartUI.SetActive(true);
        inputLocked = false;
    }

    void Update()
    {
        if (isGameOver || inputLocked) return;

        if (!gameStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                gameStarted = true;
                if (skyIntroUI != null) skyIntroUI.SetActive(false);
                if (skyCam != null) skyCam.gameObject.SetActive(false);
                
                if (introCam != null) introCam.Priority = 0;
                if (gameplayCam != null) gameplayCam.Priority = 200;

                if (pressStartUI != null) pressStartUI.SetActive(false);
                if (playerAnimator != null) playerAnimator.SetTrigger("StartGame");
            }

            return;
        }
        
        if (playerAnimator != null)
        {
            AnimatorStateInfo state = playerAnimator.GetCurrentAnimatorStateInfo(0);
            if (!animationPlayed && state.IsName("Rig|Run"))
            {
                animationPlayed = true;
                canJump = true;
                foreach (var p in allPlatforms)
                {
                    p.SetMoveDirection(Vector3.forward * platformStartSpeed);
                    if (GameTimerManager.Instance != null) GameTimerManager.Instance.StartTimer();
                }

                if (musicManager != null) musicManager.PlayGameplayMusic();
            }
        }
    }
    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        canJump = false;
        if (musicManager != null) musicManager.StopMusic();
        if (sfxSource != null && finalOuchPlayerSFX != null) sfxSource.PlayOneShot(finalOuchPlayerSFX);
        foreach (var p in allPlatforms) p.SetMoveDirection(Vector3.zero);
        StartCoroutine(GameOverSequence());
    }

    public void TriggerWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        inputLocked = true;
        canJump = false;
        canShield = false;
        if (musicManager != null) musicManager.StopMusic();
        foreach (var p in allPlatforms) p.SetMoveDirection(Vector3.zero);
        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        float timer = 0f;
        if (fadeOverlay != null) fadeOverlay.blocksRaycasts = true;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeOverlay.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        fadeOverlay.alpha = 1f;
        if (introCam) introCam.gameObject.SetActive(false);
        if (gameplayCam) gameplayCam.gameObject.SetActive(false);
        if (skyCam) skyCam.gameObject.SetActive(false);
        if (winCamera) winCamera.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeOverlay.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        fadeOverlay.alpha = 0f;
        fadeOverlay.blocksRaycasts = false;
        if (winDirector) winDirector.Play();
    }

    IEnumerator GameOverSequence()
    {
        float timer = 0f;
        if (fadeOverlay != null) fadeOverlay.blocksRaycasts = true;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float p = timer / fadeDuration;
            fadeOverlay.alpha = Mathf.Lerp(0f, 1f, p);
            if (glitchPostProcessVolume) glitchPostProcessVolume.weight = Mathf.Lerp(0f, 1f, p / glitchRampUpTime);
            yield return null;
        }

        fadeOverlay.alpha = 1f;
        if (glitchPostProcessVolume) glitchPostProcessVolume.weight = 1f;
        yield return new WaitForSeconds(0.3f);
        if (gameOverUI != null) gameOverUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(gameOverDisplayTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}