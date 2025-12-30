using UnityEngine;
using System.Collections;

public class PlayerShield : MonoBehaviour
{
    [Header("Referências")]
    public Transform shieldObject;
    public Collider shieldCollider;
    private Renderer shieldRenderer;
    private Material shieldMaterial;
    private Color baseShieldColor;

    [Header("Rotação do Shield")]
    public Vector3 shieldRotationOffset = new Vector3(-90f, 0f, 0f);

    [Header("Configuração")]
    public float holdThreshold = 0.10f;
    public Vector3 shieldOffset = new Vector3(0, 1.0f, 1.0f);

    [Header("SFX Shield")]
    public AudioSource shieldLoopSource;
    public AudioSource shieldOneShotSource;
    public AudioClip shieldOnSFX;
    public AudioClip shieldOffSFX;

    [Header("Fade")]
    [SerializeField] private float shieldFadeOutTime = 0.2f;
    [SerializeField] private float shieldFadeInTime = 0.1f;

    [SerializeField] private string colorPropertyName = "_Emissive_Color";

    private Coroutine shieldAudioFadeCoroutine;
    private Coroutine shieldVisualFadeCoroutine;
    private float shieldBaseVolume;

    [Header("Animações Bloqueantes")]
    public string[] blockedAnimations = { "StandUp", "Idle_Start" };

    private bool shieldActive;
    private bool isHolding;
    private float lmbDownTime;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        shieldBaseVolume = shieldLoopSource.volume;

        if (shieldObject != null)
        {
            shieldRenderer = shieldObject.GetComponent<Renderer>();
            shieldMaterial = shieldRenderer.material;
            shieldObject.gameObject.SetActive(false);
            shieldObject.gameObject.layer = LayerMask.NameToLayer("Shield");
        }

        if (shieldCollider == null && shieldObject != null)
            shieldCollider = shieldObject.GetComponent<Collider>();

        if (shieldCollider != null)
        {
            Collider[] playerColliders = GetComponentsInChildren<Collider>(true);
            foreach (Collider col in playerColliders)
                if (col != shieldCollider)
                    Physics.IgnoreCollision(shieldCollider, col);
        }

        if (shieldMaterial != null && shieldMaterial.HasProperty(colorPropertyName))
            baseShieldColor = shieldMaterial.GetColor(colorPropertyName);
    }

    void Update()
    {
        HandleShieldInput();

        if (shieldActive)
            UpdateShieldPosition();
    }

    void HandleShieldInput()
    {
        if (GameStartController.inputLocked) return;

        if (Input.GetMouseButtonDown(0))
        {
            lmbDownTime = Time.time;
            isHolding = true;
        }

        if (isHolding && Input.GetMouseButton(0))
        {
            if (!shieldActive &&
                Time.time - lmbDownTime > holdThreshold &&
                !IsInBlockedAnimation())
            {
                ToggleShield(true);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            if (shieldActive)
                ToggleShield(false);
        }
    }

    void UpdateShieldPosition()
    {
        shieldObject.position =
            transform.position + transform.TransformDirection(shieldOffset);

        shieldObject.rotation =
            transform.rotation * Quaternion.Euler(shieldRotationOffset);
    }

    public void ToggleShield(bool state)
    {
        if (shieldActive == state) return;

        shieldActive = state;

        if (animator)
            animator.SetBool("isShielding", state);

        // AUDIO
        if (state)
        {
            if (shieldAudioFadeCoroutine != null)
                StopCoroutine(shieldAudioFadeCoroutine);

            shieldLoopSource.clip = shieldOnSFX;
            shieldLoopSource.loop = true;
            shieldLoopSource.volume = 0f;
            shieldLoopSource.Play();

            shieldAudioFadeCoroutine =
                StartCoroutine(FadeAudio(shieldLoopSource, 0f, shieldBaseVolume, shieldFadeInTime));
        }
        else
        {
            if (shieldAudioFadeCoroutine != null)
                StopCoroutine(shieldAudioFadeCoroutine);

            shieldAudioFadeCoroutine =
                StartCoroutine(FadeOutAndStopAudio(shieldLoopSource, shieldFadeOutTime));

            if (shieldOffSFX)
                shieldOneShotSource.PlayOneShot(shieldOffSFX);
        }

        // VISUALS
        if (shieldVisualFadeCoroutine != null)
            StopCoroutine(shieldVisualFadeCoroutine);

        if (state)
        {
            shieldObject.gameObject.SetActive(true);
            shieldCollider.enabled = true;

            Color invisible = Color.black;
            invisible.a = 0f;
            shieldMaterial.SetColor(colorPropertyName, invisible);

            shieldVisualFadeCoroutine =
                StartCoroutine(FadeVisuals(1f, shieldFadeInTime));
        }
        else
        {
            shieldCollider.enabled = false;
            shieldVisualFadeCoroutine =
                StartCoroutine(FadeVisuals(0f, shieldFadeOutTime, true));
        }
    }

    public bool IsShieldActive() => shieldActive;

    bool IsInBlockedAnimation()
    {
        if (!animator) return false;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        foreach (string anim in blockedAnimations)
            if (info.IsName(anim)) return true;

        return false;
    }

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
        float start = source.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(start, 0f, t / duration);
            yield return null;
        }
        source.Stop();
        source.volume = start;
    }

    IEnumerator FadeVisuals(float targetAlpha, float duration, bool disableAtEnd = false)
    {
        float t = 0f;
        Color start = shieldMaterial.GetColor(colorPropertyName);
        Color end = baseShieldColor;
        end.a = targetAlpha;

        while (t < duration)
        {
            t += Time.deltaTime;
            shieldMaterial.SetColor(
                colorPropertyName,
                Color.Lerp(start, end, t / duration));
            yield return null;
        }

        shieldMaterial.SetColor(colorPropertyName, end);

        if (disableAtEnd)
            shieldObject.gameObject.SetActive(false);
    }
}
