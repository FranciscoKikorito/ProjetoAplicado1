using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroScene : MonoBehaviour
{
    public float totalDuration = 3.0f;
    
    public float fadeDuration = 1.0f;
    
    public string nextSceneName = "SceneFinal"; 
    
    public CanvasGroup fadeOverlay;

    void Start()
    {
        MusicManager music = FindObjectOfType<MusicManager>();
        
        if (music != null)
        {
            music.StopMusic();
        }
        
        if (fadeOverlay != null) 
            fadeOverlay.alpha = 0f;

        StartCoroutine(WaitAndLoadScene());
    }

    IEnumerator WaitAndLoadScene()
    {
        float waitTime = totalDuration - fadeDuration;
        if (waitTime > 0)
            yield return new WaitForSeconds(waitTime);
        
        if (fadeOverlay != null)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadeOverlay.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                yield return null;
            }
            fadeOverlay.alpha = 1f; 
        }
        
        SceneManager.LoadScene(nextSceneName);
    }
}