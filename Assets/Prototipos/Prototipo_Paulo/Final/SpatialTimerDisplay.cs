using UnityEngine;
using TMPro;

public class SpatialTimerDisplay : MonoBehaviour
{
    [Header("Referências de Texto")]
    public TMP_Text timerText;

    [Header("Cores do Timer")]
    public Color normalColor = Color.white;
    public Color finalSecondsColor = Color.green;
    [Range(0, 60)]
    public float changeColorAtSeconds = 10f;

    [Header("Configurações de Rotação")]
    public bool billboardToCamera = true;
    public bool lockRotationUp = false;

    private Camera mainCam;
    private Vector3 initialScale;
    void Start()
    {
        mainCam = Camera.main;
        initialScale = transform.localScale;
        
        if (timerText == null)
            timerText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        UpdateTimerVisuals();
    }

    void LateUpdate()
    {
        Mirroring();

        if (billboardToCamera)
            Billboarding();
        else if (lockRotationUp)
            LockUp();
    }
    
    void UpdateTimerVisuals()
    {
        if (GameTimerManager.Instance == null) return;
        
        if (timerText != null)
        {
            timerText.text = GameTimerManager.Instance.GetFormattedTime();
            
            float timeLeft = GameTimerManager.Instance.CurrentTime;

            if (timeLeft <= changeColorAtSeconds && timeLeft > 0)
            {
                timerText.color = finalSecondsColor;
            }
            else
            {
                timerText.color = normalColor; 
            }
        }
    }
    void Mirroring()
    {
        Vector3 parentLossyScale = transform.parent != null ? transform.parent.lossyScale : Vector3.one;
        Vector3 newScale = initialScale;

        if (parentLossyScale.x < 0) newScale.x *= -1;
        if (parentLossyScale.y < 0) newScale.y *= -1;
        if (parentLossyScale.z < 0) newScale.z *= -1;

        transform.localScale = newScale;
    }

    void Billboarding()
    {
        if (mainCam == null) return;
        transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
                         mainCam.transform.rotation * Vector3.up);
    }

    void LockUp()
    {
        Vector3 currentEuler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, currentEuler.y, 0);
    }
}