using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinSceneController : MonoBehaviour
{
    [Header("UI")] public RectTransform winUI;
    public CanvasGroup fadeOverlay;

    [Header("Timings")] public float delayBeforeShow = 5f;
    public float fadeDuration = 2.5f;
    public float slamDuration = 0.3f;

    [Header("Restart")] public float winDisplayTime = 3f;
    public string restartSceneName = "SceneFinal";
    
    [Header("Audio")] 
    public AudioSource winSource; 
    public AudioClip winSfx;
    
    private Vector2 originalPos;

    void Start()
    {
        if (winUI != null)
        {
            originalPos = winUI.anchoredPosition;
            winUI.gameObject.SetActive(false);
        }

        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            fadeOverlay.blocksRaycasts = true;
        }
        
        if (winSource != null && winSfx != null)
        {
            winSource.PlayOneShot(winSfx);
        }
        
        StartCoroutine(WinFlow());
    }

    IEnumerator WinFlow()
    {
        yield return new WaitForSeconds(delayBeforeShow);

        // Fade to black (suave)
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float progress = t / fadeDuration;
            fadeOverlay.alpha = Mathf.SmoothStep(0f, 1f, progress);
            yield return null;
        }

        fadeOverlay.alpha = 1f;

        // Mostrar UI "You reached freedom"
        if (winUI != null)
        {
            winUI.gameObject.SetActive(true);

            Vector3 startScale = Vector3.one * 4f;
            Vector3 endScale = Vector3.one;

            t = 0f;
            while (t < slamDuration)
            {
                t += Time.unscaledDeltaTime;
                winUI.localScale = Vector3.Lerp(startScale, endScale, t / slamDuration);
                yield return null;
            }

            winUI.localScale = endScale;
        }
        
        yield return new WaitForSeconds(winDisplayTime);
        SceneManager.LoadScene(restartSceneName);
    }
}
