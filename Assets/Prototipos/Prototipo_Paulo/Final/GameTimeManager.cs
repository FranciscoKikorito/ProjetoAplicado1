using UnityEngine;
using TMPro; // Precisas disto para textos se quiseres
using UnityEngine.SceneManagement;

public class GameTimerManager : MonoBehaviour
{
    public static GameTimerManager Instance; // Singleton para os prefabs encontrarem fácil

    [Header("Configuração")]
    public float totalTimeInSeconds = 60f; // __ Segundos 
    public bool isTimerRunning = false;

    [Header("Referências")]
    public GameStartController gameController; // Para chamar o Game Over
    
    private float currentTime;
    public float CurrentTime { get { return currentTime; } }

    void Awake()
    {
        // Configuração Singleton simples
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentTime = totalTimeInSeconds;
        
        if (gameController == null)
            gameController = FindObjectOfType<GameStartController>();
    }

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    void Update()
    {
        if (isTimerRunning && currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                isTimerRunning = false;
                TimeIsUp();
            }
        }
    }

    void TimeIsUp()
    {
        Debug.Log("O Tempo Acabou!");
        if (gameController != null)
        {
            gameController.TriggerGameOver();
        }
    }
    public string GetFormattedTime()
    {
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        
        // Retorna formato "02:45"
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
