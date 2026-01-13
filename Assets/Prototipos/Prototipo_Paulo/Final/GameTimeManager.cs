using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class GameTimerManager : MonoBehaviour
{
    public static GameTimerManager Instance; // Singleton para os prefabs encontrarem fácil

    [Header("Configuração")]
    public float totalTimeInSeconds = 180f; // __ Segundos 
    public bool isTimerRunning = false;

    [Header("Cinematic Settings")]
    public PlayableDirector winCinematic; // 2. ADD THIS VARIABLE
    public GameObject gameplayUI;         // Optional: to hide the timer during the movie

    private bool _hasWon = false;

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
        if (!isTimerRunning || currentTime <= 0)
            return;
        
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
        if (totalTimeInSeconds <= 0 && !_hasWon)
        {
            TriggerWin();
        }
    }
    void TimeIsUp()
    {
            Debug.Log("O Tempo Acabou! — VITÓRIA");
            if (gameController != null)
            {
                gameController.TriggerWin();
            }
    }
    public string GetFormattedTime()
    {
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        
        // Retorna formato "02:45"
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void TriggerWin()
    {
        _hasWon = true;
        totalTimeInSeconds = 0;

        // 4. STOP THE GAMEPLAY
        // You can use your 'Game Controller' reference here to stop player movement
        // example: gameController.StopRunner();

        if (gameplayUI != null) gameplayUI.SetActive(false);

        // 5. PLAY THE CINEMATIC
        if (winCinematic != null)
        {
            winCinematic.Play();
        }
    }
}
