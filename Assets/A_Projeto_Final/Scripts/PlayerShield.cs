using UnityEngine;
using System.Collections;

public class PlayerShield : MonoBehaviour
{
    [Header("Referências")] public Transform shieldObject;
    public Collider shieldCollider;
    private Renderer shieldRenderer;
    private Color baseShieldColor;

    [Header("Rotação do Shield")] public Vector3 shieldRotationOffset = new Vector3(-90f, 0f, 0f);

    [Header("Configuração")] public float holdThreshold = 0.10f;
    public Vector3 shieldOffset = new Vector3(0, 1.0f, 1.0f);

    [Header("SFX Shield")] public AudioSource shieldLoopSource;
    public AudioSource shieldOneShotSource;
    public AudioClip shieldOnSFX;
    public AudioClip shieldOffSFX;

    [Header("Configurações de Fade")] [SerializeField]
    private float shieldFadeOutTime = 0.2f;

    [SerializeField] private float shieldFadeInTime = 0.1f;

    // --- CORREÇÃO AQUI: Nome da propriedade no Shader Graph ---
    [SerializeField] private string colorPropertyName = "_Emissive_Color";

    private Coroutine shieldAudioFadeCoroutine;
    private Coroutine shieldVisualFadeCoroutine;
    private float shieldBaseVolume;

    [Header("Animações Bloqueantes")] public string[] blockedAnimations = { "StandUp", "Idle_Start" };

    private bool shieldActive = false;
    private float lmbDownTime;
    private bool isHolding = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        shieldBaseVolume = shieldLoopSource.volume;

        if (shieldObject != null)
        {
            shieldRenderer = shieldObject.GetComponent<Renderer>();
            shieldObject.gameObject.SetActive(false);
            shieldObject.gameObject.layer = LayerMask.NameToLayer("Shield");
        }

        if (shieldCollider == null && shieldObject != null)
            shieldCollider = shieldObject.GetComponent<Collider>();

        if (shieldCollider != null)
        {
            Collider[] playerColliders = GetComponentsInChildren<Collider>(true);
            foreach (Collider col in playerColliders)
            {
                if (col != shieldCollider)
                    Physics.IgnoreCollision(shieldCollider, col);
            }
        }

        if (shieldRenderer != null && shieldRenderer.material.HasProperty(colorPropertyName))
        {
            baseShieldColor = shieldRenderer.material.GetColor(colorPropertyName);
        }
    }

    void Update()
    {
        HandleShieldInput();

        if (shieldActive)
            UpdateShieldPosition();
    }

    private void HandleShieldInput()
    {
        if (GameStartController.inputLocked) return;

        if (Input.GetMouseButtonDown(0))
        {
            lmbDownTime = Time.time;
            isHolding = true;
        }

        if (isHolding && Input.GetMouseButton(0))
        {
            if (!shieldActive && Time.time - lmbDownTime > holdThreshold && !IsInBlockedAnimation())
                ToggleShield(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            if (shieldActive)
                ToggleShield(false);
        }
    }

    private void UpdateShieldPosition()
    {
        if (shieldObject == null) return;
        shieldObject.position = transform.position + transform.TransformDirection(shieldOffset);
        shieldObject.rotation = transform.rotation * Quaternion.Euler(shieldRotationOffset);
    }

    public void ToggleShield(bool state)
    {
        if (shieldActive == state) return;

        shieldActive = state;

        if (animator != null)
            animator.SetBool("isShielding", state);

        // Áudio
        if (state)
        {
            if (shieldAudioFadeCoroutine != null) StopCoroutine(shieldAudioFadeCoroutine);
            shieldLoopSource.clip = shieldOnSFX;
            shieldLoopSource.loop = true;
            shieldLoopSource.time = Random.Range(0f, shieldOnSFX.length);
            shieldLoopSource.volume = 0f;
            shieldLoopSource.Play();
            shieldAudioFadeCoroutine =
                StartCoroutine(FadeAudio(shieldLoopSource, 0f, shieldBaseVolume, shieldFadeInTime));
        }
        else
        {
            if (shieldAudioFadeCoroutine != null) StopCoroutine(shieldAudioFadeCoroutine);
            shieldAudioFadeCoroutine = StartCoroutine(FadeOutAndStopAudio(shieldLoopSource, shieldFadeOutTime));
            if (shieldOffSFX != null) shieldOneShotSource.PlayOneShot(shieldOffSFX);
        }

   
        if (shieldVisualFadeCoroutine != null) StopCoroutine(shieldVisualFadeCoroutine);

        if (state) // Fade In 
        {
            if (shieldObject != null)
            {
                bool wasInactive = !shieldObject.gameObject.activeSelf;
                
                shieldObject.gameObject.SetActive(true);
                
                if (wasInactive && shieldRenderer != null)
                {
                    Color invisibleColor = Color.black; 
                    invisibleColor.a = 0f;
                    shieldRenderer.material.SetColor(colorPropertyName, invisibleColor);
                }
            }

            if (shieldCollider != null) shieldCollider.enabled = true;
            shieldVisualFadeCoroutine = StartCoroutine(FadeVisuals(1f, shieldFadeInTime));
        }
        else // Fade Out 
        {
            if (shieldCollider != null) shieldCollider.enabled = false;
            
            shieldVisualFadeCoroutine = StartCoroutine(FadeVisuals(0f, shieldFadeOutTime, true));
        }
    }

    public bool IsShieldActive() => shieldActive;

    private bool IsInBlockedAnimation()
    {
        if (animator == null) return false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        foreach (var anim in blockedAnimations)
        {
            if (stateInfo.IsName(anim)) return true;
        }

        return false;
    }

    // --- COROUTINES ---

    IEnumerator FadeAudio(AudioSource source, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        source.volume = to;
    }

    IEnumerator FadeOutAndStopAudio(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    } 
    IEnumerator FadeVisuals(float targetAlpha, float duration, bool disableObjectAtEnd = false)
    {
        if (shieldRenderer == null) yield break;

        float t = 0f;
        Color startColor = shieldRenderer.material.GetColor(colorPropertyName);
        
        Color endColor = baseShieldColor;
        
        if (targetAlpha < 0.01f)
        {
            endColor = Color.black;
            endColor.a = 0f;
        }
        else
        {
            endColor.a = targetAlpha;
        }

        while (t < duration)
        {
            t += Time.deltaTime;
            Color currentColor = Color.Lerp(startColor, endColor, t / duration);
            shieldRenderer.material.SetColor(colorPropertyName, currentColor);
            yield return null;
        }

        shieldRenderer.material.SetColor(colorPropertyName, endColor);

        if (disableObjectAtEnd && shieldObject != null)
        {
            shieldObject.gameObject.SetActive(false);
        }
    }
}