using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource introSource;
    public AudioSource gameplaySource;

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;
    public float musicVolume = 0.07f;

    private Coroutine fadeCoroutine;

    void Start()
    {
        PlayIntroMusic();
    }

    public void PlayIntroMusic()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        introSource.volume = musicVolume;
        introSource.Play();

        gameplaySource.Stop();
    }
    public void PlayGameplayMusic()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(CrossFade(introSource, gameplaySource));
    }
    
    public void StopMusic()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        
        introSource.Stop();
        gameplaySource.Stop();
    }
    IEnumerator CrossFade(AudioSource from, AudioSource to)
    {
        if (!to.isPlaying)
        {
            to.volume = 0f;
            to.Play();
        }

        float t = 0f;
        float fromStart = from.volume;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float k = t / fadeDuration;

            from.volume = Mathf.Lerp(fromStart, 0f, k);
            to.volume = Mathf.Lerp(0f, musicVolume, k);

            yield return null;
        }

        from.Stop();  
        to.volume = musicVolume;
    }
}