using UnityEngine;
using System.Collections;

public class PlayerShield : MonoBehaviour
{
    [Header("Referências")]
    public Transform shieldObject;
    public Collider shieldCollider;
    private Renderer shieldRenderer;
    private Material shieldMaterial; // store material instance
    private Color baseShieldColor;

    [Header("Rotação do Shield")]
    public Vector3 shieldRotationOffset = new Vector3(-90f, 0f, 0f);

    [Header("Configuração")]
    public float holdThreshold = 0.10f;
    public Vector3 shieldOffset = new Vector3(0, 1.0f, 1.0f);

    [Header("SFX Shield")]
    public AudioSource shieldOneShotSource;
    public AudioClip shieldOnSFX;
    public AudioClip shieldOffSFX;

    [Header("Configurações de Fade")]
    [SerializeField] private float shieldFadeOutTime = 0.2f;
    [SerializeField] private float shieldFadeInTime = 0.2f;

    [SerializeField] private string colorPropertyName = "_Emissive_Color";

    private Coroutine shieldVisualFadeCoroutine;

    [Header("Animações Bloqueantes")]
    public string[] blockedAnimations = { "StandUp", "Idle_Start" };

    private bool shieldActive = false;
    private float lmbDownTime;
    private bool isHolding = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (shieldObject != null)
        {
            shieldRenderer = shieldObject.GetComponent<Renderer>();
            shieldMaterial = shieldRenderer.material; // store instance
            shieldObject.gameObject.SetActive(false);
            shieldObject.gameObject.layer = LayerMask.NameToLayer("Shield");

            if (shieldMaterial.HasProperty(colorPropertyName))
                baseShieldColor = shieldMaterial.GetColor(colorPropertyName);
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

        // AUDIO - just play one-shot for shield on
        if (state)
        {
            if (shieldOneShotSource != null && shieldOnSFX != null)
            {
                shieldOneShotSource.PlayOneShot(shieldOnSFX);
            }
        }
        else
        {
            if (shieldOneShotSource != null && shieldOffSFX != null)
                shieldOneShotSource.PlayOneShot(shieldOffSFX);
        }

        // VISUALS
        if (shieldVisualFadeCoroutine != null) StopCoroutine(shieldVisualFadeCoroutine);

        if (state) // Fade In
        {
            if (shieldObject != null)
            {
                bool wasInactive = !shieldObject.gameObject.activeSelf;
                shieldObject.gameObject.SetActive(true);

                if (wasInactive && shieldMaterial != null)
                {
                    Color invisibleColor = Color.black;
                    invisibleColor.a = 0f;
                    shieldMaterial.SetColor(colorPropertyName, invisibleColor);
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
            if (stateInfo.IsName(anim)) return true;

        return false;
    }

    // --- COROUTINES ---
    IEnumerator FadeVisuals(float targetAlpha, float duration, bool disableObjectAtEnd = false)
    {
        if (shieldMaterial == null) yield break;

        float t = 0f;
        Color startColor = shieldMaterial.GetColor(colorPropertyName);
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
            shieldMaterial.SetColor(colorPropertyName, currentColor);
            yield return null;
        }

        shieldMaterial.SetColor(colorPropertyName, endColor);

        if (disableObjectAtEnd && shieldObject != null)
            shieldObject.gameObject.SetActive(false);
    }
}
