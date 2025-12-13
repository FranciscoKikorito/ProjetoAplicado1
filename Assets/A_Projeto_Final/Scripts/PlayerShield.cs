using UnityEngine;
using System.Collections;
public class PlayerShield : MonoBehaviour
{
    [Header("Referências")] public Transform shieldObject;
    public Collider shieldCollider;

    [Header("Rotação do Shield")] public Vector3 shieldRotationOffset = new Vector3(-90f, 0f, 0f);

    [Header("Configuração")] public float holdThreshold = 0.25f;
    public Vector3 shieldOffset = new Vector3(0, 1.0f, 1.0f);

    [Header("SFX Shield")] public AudioSource shieldLoopSource;
    public AudioSource shieldOneShotSource;
    public AudioClip shieldOnSFX;
    public AudioClip shieldOffSFX;
    [SerializeField] private float shieldFadeOutTime = 0.2f;
    [SerializeField] private float shieldFadeInTime = 0.1f;

    private Coroutine shieldFadeCoroutine;
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
            shieldObject.gameObject.SetActive(false);

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

        if (shieldObject != null)
            shieldObject.gameObject.layer = LayerMask.NameToLayer("Shield");
    }

    void Update()
    {
        HandleShieldInput();

        if (shieldActive)
            UpdateShieldPosition();
    }

    private void HandleShieldInput()
    {
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

        shieldObject.position =
            transform.position + transform.TransformDirection(shieldOffset);

        shieldObject.rotation =
            transform.rotation * Quaternion.Euler(shieldRotationOffset);
    }

    public void ToggleShield(bool state)
    {
        if (shieldActive == state) return;

        shieldActive = state;

        if (shieldObject != null)
            shieldObject.gameObject.SetActive(state);

        if (animator != null)
            animator.SetBool("isShielding", state);

        if (state)
        {
            if (shieldFadeCoroutine != null)
                StopCoroutine(shieldFadeCoroutine);

            shieldLoopSource.clip = shieldOnSFX;
            shieldLoopSource.loop = true;

            // random start
            shieldLoopSource.time = Random.Range(0f, shieldOnSFX.length);
            shieldLoopSource.volume = 0f;

            shieldLoopSource.Play();

            shieldFadeCoroutine = StartCoroutine(FadeAudio(
                shieldLoopSource, 0f, shieldBaseVolume, shieldFadeInTime));
        }
        else
        {
            if (shieldFadeCoroutine != null)
                StopCoroutine(shieldFadeCoroutine);

            shieldFadeCoroutine = StartCoroutine(FadeOutAndStop(shieldLoopSource, shieldFadeOutTime));

            if (shieldOffSFX != null)
                shieldOneShotSource.PlayOneShot(shieldOffSFX);
        }
    }
    public bool IsShieldActive() => shieldActive;
    private bool IsInBlockedAnimation()
    {
        if (animator == null) return false;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        foreach (var anim in blockedAnimations)
        {
            if (stateInfo.IsName(anim))
                return true;
        }
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

    IEnumerator FadeOutAndStop(AudioSource source, float duration)
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
        source.volume = startVolume; // prepara para próxima ativação
    }
}